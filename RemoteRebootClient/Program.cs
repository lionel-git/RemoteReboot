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
                var hostname = ConfigurationManager.AppSettings["Hostname"].ToString();
                var port = int.Parse(ConfigurationManager.AppSettings["ServicePort"]);
                var token = ConfigurationManager.AppSettings["Token"].ToString();
                if (string.IsNullOrWhiteSpace(token))
                    throw new Exception("Token is null or empty, set it to a value!!!");

                using (var client = new TcpClient())
                {
                    client.Connect(hostname, port);
                    client.Client.Send(Encoding.ASCII.GetBytes(token));
                    var buffer = new byte[1024];
                    int r = client.Client.Receive(buffer);
                    var ret = Encoding.ASCII.GetString(buffer, 0, r);
                    Console.WriteLine($"Received: {ret}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
