using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RemoteReboot
{
    class Program
    {
        [DllImport("ShutdownWrapper.dll", CharSet = CharSet.Unicode)]
        public static extern bool MySystemReboot(bool forceAppsClosed, bool dryRun);

        static void Main(string[] args)
        {
            try
            {
                bool forceAppsClosed = true;
                bool dryRun = true;
                bool ret = MySystemReboot(forceAppsClosed, dryRun);
                Console.WriteLine($"Return: {ret}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
