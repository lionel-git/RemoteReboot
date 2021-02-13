using log4net;
using ServiceUtils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

        private volatile bool _running = true;

        public bool ConsoleMode { get; set; }

        private void MainLoop()
        {
            while (_running)
            {
               
                Thread.Sleep(1000);
            }
            _logger.Info("Exiting main loop");
        }

        private void InitConfig(string[] args)
        {
            _port = int.Parse(ConfigurationManager.AppSettings["ServicePort"]);
            _token = ConfigurationManager.AppSettings["Token"].ToString();
            if (string.IsNullOrWhiteSpace(_token))
                throw new Exception("Token is null or empty, set it to a value!!!");
        }

        public void OnStart(string[] args)
        {
            _logger.Info("=====================================================");
            InitConfig(args);
        }

        public void OnStop()
        {
            _logger.Info("Exiting On Stop");
        }
    }
}
