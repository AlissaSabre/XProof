using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Ionic.Zip;

namespace XProof
{
    class Transformer
    {
        private static readonly XNamespace SDL = XNamespace.Get("http://sdl.com/FileTypes/SdlXliff/1.0");
        private static readonly XNamespace IWS = XNamespace.Get("http://www.idiominc.com/ws/asset");
        private static readonly XNamespace MQ = XNamespace.Get("MQXliff"); // This is NOT a placeholder; it's real!  :(

        private static readonly XNamespace W = XNamespace.Get("http://schemas.openxmlformats.org/wordprocessingml/2006/main");

        private static XName XML_LANG = XNamespace.Xml + "lang";
        private static XName XML_SPACE = XNamespace.Xml + "space";

        private readonly XElement Doc;

        private XNamespace X;

        public Transformer()
        {
            Doc = new XElement(W + "document",
                new XAttribute(XNamespace.Xmlns + "w", W.NamespaceName),
                new XElement(W + "body"));
        }

        public void Feed(string filename)
        {
            var read = false;
            using (var stream = File.OpenRead(filename))
            {
                if (IsZipStream(stream))
                {
                    using (var zip = ZipFile.Read(stream))
                    {
                        foreach (var entry in zip.Entries.Where(e => !e.IsDirectory))
                        {
                            using (var content = entry.OpenReader())
                            {
                                read |= TryFeedStream(content);
                            }
                        }
                    }
                }
                else
                {
                    read = TryFeedStream(stream);
                }
            }
            if (!read)
            {
                throw new IOException("Unrecognized file format: " + filename);
            }
        }

        private bool IsZipStream(Stream stream)
        {
            long pos = stream.Seek(0, SeekOrigin.Current);
            try
            {
                var header = new byte[4];
                if (stream.Read(header, 0, header.Length) != header.Length) return false;
                return header[0] == 'P' && header[1] == 'K' && header[2] == 3 && header[3] == 4;
            }
            finally
            {
                stream.Seek(pos, SeekOrigin.Begin);
            }
        }

        private bool TryFeedStream(Stream stream)
        {
            XElement xliff;
            try
            {
                xliff = XElement.Load(stream);
                if (xliff.Name.LocalName != "xliff") return false;
                X = xliff.Name.Namespace;
            }
            catch (Exception)
            {
                return false;
            }

            Doc.Element(W + "body").Add(xliff.Elements(X + "file").SelectMany(TransFile));
            return true;
        }

        private IEnumerable<XElement> TransFile(XElement file)
        {
            var original = Path.GetFileName((string)file.Attribute("original") ?? (string)file.Descendants(IWS + "ais_src_path").FirstOrDefault() ?? "");
            var lang = (string)file.Attribute("target-language");

            yield return new XElement(W + "tbl",
                new XElement(W + "tblPr",
                    new XElement(W + "tblStyle", new XAttribute(W + "val", "Table")),
                    new XElement(W + "tblW", new XAttribute(W + "w", "0"), new XAttribute(W + "type", "auto"))),
                new XElement(W + "tblGrid",
                    new XElement(W + "gridCol"),
                    new XElement(W + "gridCol"),
                    new XElement(W + "gridCol"),
                    new XElement(W + "gridCol")),
                new XElement(W + "tr",
                    new XElement(W + "tc", Wprt("Asset")),
                    new XElement(W + "tc", Wprt("Seg")),
                    new XElement(W + "tc", Wprt("Source")),
                    new XElement(W + "tc", Wprt("Target"))),
                file.Descendants(X + "trans-unit")
                    .Where(tu => (string)tu.Attribute("translate") != "no" && !string.IsNullOrWhiteSpace((string)tu.Element(X + "source")))
                    .SelectMany(tu => TransTU(original, tu, lang)));
            
            yield return new XElement(W + "sectPr", 
                new XElement(W + "pgSz",
                    new XAttribute(W + "w", 16839),
                    new XAttribute(W + "h", 11907),
                    new XAttribute(W + "orient", "landscape")),
                new XElement(W + "pgMar",
                    new XAttribute(W + "top", 144),
                    new XAttribute(W + "right", 144),
                    new XAttribute(W + "bottom", 144),
                    new XAttribute(W + "left", 144),
                    new XAttribute(W + "header", 72),
                    new XAttribute(W + "footer", 72),
                    new XAttribute(W + "gutter", 0)));
        }

        private IEnumerable<XElement> TransTU(string original, XElement tu, string lang)
        {
            lang = lang ?? (string)tu.Elements(X + "target").Attributes(XML_LANG).FirstOrDefault();
            var segs = tu.Elements(X + "seg-source").Descendants(X + "mrk").Where(mrk => (string)mrk.Attribute("mtype") == "seg");
            if (segs.Any())
            {
                var tsegs = new Dictionary<string, XElement>();
                foreach (var m in tu.Elements(X + "target").Descendants(X + "mrk").Where(mrk => (string)mrk.Attribute("mtype") == "seg"))
                {
                    tsegs.Add((string)m.Attribute("mid") ?? "", m);
                }
                foreach (var s in segs)
                {
                    var id = (string)s.Attribute("mid");
                    XElement t;
                    if (!tsegs.TryGetValue(id, out t)) t = null;
                    yield return TransPair(original, id, s, t, lang);
                }
            }
            else
            {
                yield return TransPair(original, (string)tu.Attribute("id"), tu.Element(X + "source"), tu.Element(X + "target"), lang);
            }
        }

        private XElement TransPair(string original, string id, XElement source, XElement target, string lang)
        {
            return new XElement(W + "tr",
                new XElement(W + "tc", Wprt(original)),
                new XElement(W + "tc", Wprt(id)),
                new XElement(W + "tc", Wprt(TransElement(source))),
                new XElement(W + "tc", Wprt(TransElement(target), lang)));
        }

        private string TransElement(XElement elem)
        {
            if (elem == null) return "";
            var sb = new StringBuilder();
            Extract(sb, elem);
            return sb.ToString();
        }

        private static readonly string[] IGNORES = { "x", "bx", "ex", "bpt", "ept", "ph", "it", "sub" };

        private void Extract(StringBuilder sb, XElement elem)
        {
            foreach (var node in elem.Nodes())
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.Text:
                        sb.Append(((XText)node).Value);
                        break;
                    case XmlNodeType.Element:
                        var e = (XElement)node;
                        if (e.Name.Namespace == X && Array.IndexOf(IGNORES, e.Name.LocalName) >= 0)
                        {
                            // Ignore this element as well as its entire content.
                        }
                        else
                        {
                            // Extract flat text from the contents, dropping opening and closing tags.
                            Extract(sb, e);
                        }
                        break;
                    default:
                        // Ignore any other nodes, e.g., comments and PI.
                        break;
                }
            }
        }

        private XElement Wprt(string text)
        {
            return
                new XElement(W + "p",
                    new XElement(W + "r",
                        new XElement(W + "rPr",
                            new XElement(W + "noProof")),
                        new XElement(W + "t",
                            new XAttribute(XML_SPACE, "preserve"), text)));
        }

        private XElement Wprt(string text, string lang)
        {
            return
                new XElement(W + "p",
                    new XElement(W + "r",
                        lang == null ? null : 
                            new XElement(W + "rPr",
                                new XElement(W + "lang",
                                    new XAttribute(W + "val", lang))),
                        new XElement(W + "t",
                            new XAttribute(XML_SPACE, "preserve"), text)));
        }

        public void SaveTo(string filename)
        {
            byte[] bytes;
            using (var wpml = new MemoryStream())
            {
                Doc.Save(wpml, SaveOptions.DisableFormatting);
                bytes = wpml.ToArray();
            }
            using (var zip = ZipFile.Read(GetSupportFilePath("Empty.docx")))
            {
                zip.RemoveEntry("word/document.xml");
                zip.AddEntry("word/document.xml", bytes);
                zip.Save(filename);
            }
        }

        private static string GetSupportFilePath(string name)
        {
            return Path.Combine(Path.GetDirectoryName(typeof(Transformer).Assembly.Location), name);
        }
    }
}
