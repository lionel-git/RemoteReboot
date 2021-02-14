using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RemoteRebootClient
{
    class Program
    {
        static void GetGuid()
        {
            var guid = Guid.NewGuid();
            Console.WriteLine(guid);
        }

        static void Main(string[] args)
        {
            try
            {
                // GetGuid(); return;
                string token;
                if (args.Length == 1)
                    token = args[0];
                else
                    token = ConfigurationManager.AppSettings["Token"].ToString();
                var hostname = ConfigurationManager.AppSettings["Hostname"].ToString();
                var port = int.Parse(ConfigurationManager.AppSettings["ServicePort"]);
                Console.WriteLine($"Connecting to {hostname}:{port}..");
                using (var client = new TcpClient())
                {
                    client.Connect(hostname, port);
                    client.Client.Send(Encoding.ASCII.GetBytes(token));
                    var buffer = new byte[1024];
                    int length = client.Client.Receive(buffer);
                    var reply = Encoding.ASCII.GetString(buffer, 0, length);
                    Console.WriteLine($"Reply: {reply}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
