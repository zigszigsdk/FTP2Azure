using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using AzureFtpServer.Ftp;
using AzureFtpServer.Azure;

namespace FTPServerWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private FtpServer _server;

        public override bool OnStart()
        {
            if (_server == null)
                _server = new FtpServer();

            _server.NewConnection += ServerNewConnection;
            _server.UserLogoutEvent += ServerLogoutEvent;
            _server.errorHandler += ErrorHandle;

            return base.OnStart();
        }

        public override void Run()
        {
            while (true)
            {
                if (_server.Started)
                {
                    Thread.Sleep(10000);
                    Trace.WriteLine("Server is alive.", "Information");
                }
                else
                {
                    _server.Start();
                    Trace.WriteLine("Server starting.", "Control");
                }
            }
        }

        void ServerNewConnection(int nId)
        {
            Trace.WriteLine(String.Format("Connection {0} accepted", nId), "Connection");
        }

        void ServerLogoutEvent(string username)
        {
            Trace.WriteLine(String.Format("user {0} logged out", username), "Connection");
        }

        void ErrorHandle(Exception e) 
        {
            Trace.WriteLine("Error in FTP: " + e.Message);
            throw e;
        }
    }
}
