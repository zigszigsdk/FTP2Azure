using System;
using System.Net;
using System.Net.Sockets;
using AzureFtpServer.Ftp;
using AzureFtpServer.General;
using Microsoft.WindowsAzure.ServiceRuntime;
using AzureFtpServer.Provider;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// PASV command handler
    /// enter passive mode
    /// </summary>
    internal class PasvCommandHandler : FtpCommandHandler
    {
        private int m_nPort;

        public PasvCommandHandler(FtpConnectionObject connectionObject)
            : base("PASV", connectionObject)
        {
            m_nPort = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["FTP2Azure.Passive"].IPEndpoint.Port;

                
        }

        protected override string OnProcess(string sMessage)
        {
            ConnectionObject.DataConnectionType = DataConnectionType.Passive;

            string pasvListenAddress = GetPassiveAddressInfo();

            //return GetMessage(227, string.Format("Entering Passive Mode ({0})", pasvListenAddress));
            TcpListener listener;
            if(StorageProviderConfiguration.Mode == Modes.Live)
                listener = SocketHelpers.CreateTcpListener(
                    RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["FTP2Azure.Passive"].IPEndpoint);
            else
                listener = SocketHelpers.CreateTcpListener(new IPEndPoint(
                    IPAddress.Parse("127.0.0.1"), RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["FTP2Azure.Passive"].IPEndpoint.Port));
            if (listener == null)
            {
                return GetMessage(550, string.Format("Couldn't start listener on port {0}", m_nPort));
            }

            SocketHelpers.Send(ConnectionObject.Socket, string.Format("227 Entering Passive Mode ({0})\r\n", pasvListenAddress), ConnectionObject.Encoding);

            listener.Start();

            ConnectionObject.PassiveSocket = listener.AcceptTcpClient();

            listener.Stop();

            return "";
        }

        private string GetPassiveAddressInfo()
        {
            // get local ipv4 ip
            IPAddress ipAddress = SocketHelpers.GetLocalAddress();
            if (ipAddress == null)
                throw new Exception("The ftp server do not have a ipv4 address");
            string retIpPort = ipAddress.ToString();
            retIpPort = retIpPort.Replace('.', ',');

            // append the port
            retIpPort += ',';
            retIpPort += (m_nPort / 256).ToString();
            retIpPort += ',';
            retIpPort += (m_nPort % 256).ToString();

            return retIpPort;
        }
    }
}