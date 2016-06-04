using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XProof
{
    public class Proofreader : IDisposable
    {
        private readonly List<string> TempFilenames = new List<string>();

        public void Dispose()
        {
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

            using (var word = new Word())
            {
                word.Proofread(tmp);
            }
        }
    }
}
