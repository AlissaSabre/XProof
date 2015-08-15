using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NetOffice.WordApi;
using NetOffice.WordApi.Enums;

namespace XProof
{
    class Proofreader : IDisposable
    {
        private Word Word = new Word();

        private readonly List<string> TempFilenames = new List<string>();

        public void Dispose()
        {
            Word.Dispose();
            foreach (var f in TempFilenames)
            {
                try
                {
                    File.Delete(f);
                }
                catch (Exception) { }
            }
        }

        public void DoProof(string[] filenames)
        {
            var tmp = Path.GetTempFileName();
            {
                var t2 = tmp + ".docx";
                File.Move(tmp, t2);
                tmp = t2;
            }
            TempFilenames.Add(tmp);

            var tr = new Transformer();
            foreach (var f in filenames)
            {
                tr.Feed(f);
            }
            tr.SaveTo(tmp);

            Word.Open(tmp);
        }

        public void Shutdown()
        {
            Word.Shutdown();
        }
    }
}
