using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XProof;

namespace XProofCmd
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Out.WriteLine("Invalid command line parameters.  Usage: XProofCmd filename...");
                return 33;
            }

            try
            {
                using (var proof = new Proofreader())
                {
                    proof.DoProof(args);
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.ToString());
                return 1;
            }
        }
    }
}
