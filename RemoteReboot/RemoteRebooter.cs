using log4net;
using ServiceUtils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteReboot
{
    public class RemoteRebooter : IService
    {
        private static readonly ILog _logger = LogManager.GetLogger("RemoteRebooter");

        private int _port;
        private string _token;
        private bool _dryRun = true;
        private bool _forceAppsClosed = true;

        private volatile bool _running = true;

        private Thread _thread;

        public bool ConsoleMode { get; set; }

        [DllImport("ShutdownWrapper.dll", CharSet = CharSet.Unicode)]
        public static extern bool MySystemReboot(bool forceAppsClosed, bool dryRun);

        public const string ResponseOk = "OK";
        public const string ResponseNotOk = "NOT OK";

        private byte[] GetResponse(string response)
        {
            _logger.Info($"Response: {response}");
            return Encoding.ASCII.GetBytes(response);
        }

        private void MainLoop()
        {
            _logger.Info($"Start listening on port={_port}...");
            var endPoint = new IPEndPoint(IPAddress.Any, _port);
            var tcpListener = new TcpListener(endPoint);
            tcpListener.Start();            
            while (_running)
            {
                using (var socket = tcpListener.AcceptSocket())
                {
                    _logger.Info($" Received connection from {socket.RemoteEndPoint}");
                    var buffer = new byte[1024];
                    int nbBytes = socket.Receive(buffer);
                    _logger.Info($"Received: {nbBytes} bytes");
                    var token = Encoding.ASCII.GetString(buffer, 0, nbBytes);
                    byte[] response;
                    if (token == _token)
                    {
                        response = GetResponse(ResponseOk);
                        MySystemReboot(_forceAppsClosed, _dryRun);
                    }
                    else
                    {
                        response = GetResponse(ResponseNotOk);
                    }
                    socket.Send(response);
                    socket.Shutdown(SocketShutdown.Both);
                }
            }
            _logger.Info("Exiting main loop");
        }

        private void InitConfig(string[] args)
        {
            _port = int.Parse(ConfigurationManager.AppSettings["ServicePort"]);
            _token = ConfigurationManager.AppSettings["Token"].ToString();
            _dryRun = bool.Parse(ConfigurationManager.AppSettings["DryRun"]);
            _forceAppsClosed = bool.Parse(ConfigurationManager.AppSettings["ForceAppsClosed"]);
            if (string.IsNullOrWhiteSpace(_token))
                throw new Exception("Token is null or empty, set it to a value!!!");
            _logger.Info($"Config: port={_port}, dryRun={_dryRun}, forceAppsClosed={_forceAppsClosed}");
        }

        public void OnStart(string[] args)
        {
            _logger.Info("=====================================================");
            InitConfig(args);
            _thread = new Thread(MainLoop);
            _thread.Start();
        }

        public void OnStop()
        {
            _thread.Abort();
            _logger.Info("Exiting On Stop");
        }
    }
}
