using log4net;
using ServiceUtils;
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
        private static readonly ILog _logger = LogManager.GetLogger("RemoteReboot");

        [DllImport("ShutdownWrapper.dll", CharSet = CharSet.Unicode)]
        public static extern bool MySystemReboot(bool forceAppsClosed, bool dryRun);

        static void Test1()
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

        static void Main(string[] args)
        {
            try
            {
                Starter<RemoteRebooter>.Start("RemoteReboot", args, true);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                Console.WriteLine(e);
            }
        }
    }
}
