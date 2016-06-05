using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XProof
{
    /// <summary>
    /// A sort of a wrapper of Microsoft Word to proofread (spell checking and grammer checking) XLIFF documents. 
    /// </summary>
    public class Proofreader : IDisposable
    {
        private readonly List<string> TempFilenames = new List<string>();

        /// <summary>
        /// Implements <see cref="IDisposable.Dispose"/>.
        /// </summary>
        /// <remarks>
        /// This method deletes all temporary files that this object has created and still remain.
        /// </remarks>
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
            TempFilenames.Clear();
        }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public Proofreader()
        {
            // Do nothing special.
        }

        /// <summary>
        /// Proofreads target contents of XLIFF files.
        /// </summary>
        /// <param name="filenames">An array of filesnames to proofread.
        /// A file can be an XLIFF file or a zipped set of XLIFF files.
        /// Those files other than XLIFF are ignored in ZIP, 
        /// so you can pass XLIFF based packages like wsxz.
        /// </param>
        /// <exception cref="ApplicationException">Microsoft Word is not available on the system.</exception>
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

            try
            {
                using (var word = new Word())
                {
                    word.Proofread(tmp);
                }
            }
            catch (ArgumentException e)
            {
                if (e.Message == "progId not found. Word.Application")
                {
                    throw new ApplicationException("Microsoft Word is not available.", e);
                }
                throw e;
            }
        }
    }
}
