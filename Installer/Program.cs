using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Installer
{
    class Program
    {
        // Renames a WiX created msi file to some meaningful name with main file version embedded.

        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Out.WriteLine("Usage: Installer setup.msi mainfile.exe");
                return 33;
            }
            try
            {
                var msi = args[0];
                var msiname = Path.GetFileName(msi);
                var mainfile = args[1];
                var basename = Path.GetFileNameWithoutExtension(mainfile);
                var version = FileVersionInfo.GetVersionInfo(mainfile).FileVersion;
                var targetname = string.Format("{0}-{1}-{2}", basename, version, msiname);
                var target = Path.Combine(Path.GetDirectoryName(msi), targetname);
                File.Delete(target);
                File.Move(msi, target);
                return 0;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
                return 1;
            }
        }
    }
}
