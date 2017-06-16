using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Diagnostics;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.General;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Net;
using AzureFtpServer.Azure;

namespace AzureFtpServer.Ftp
{
    /// Listens for incoming connections and accepts them.
    /// Incomming socket connections are then passed to the socket handling class (FtpSocketHandler).
    public class FtpServer : IDisposable
    {
        private readonly ArrayList m_apConnections;
        private readonly IFileSystemClassFactory m_fileSystemClassFactory;
        private int m_nId;
        private TcpListener m_socketListen;
        private Thread m_theThread;
        private bool m_started = false;
        private Encoding m_encoding;
        private int m_maxClients;


        public delegate void ConnectionHandler(int nId);

        public delegate void UserAccessEvent(string username);

        public delegate void ErrorEvent(Exception e);

        public event ConnectionHandler ConnectionClosed;
        public event ConnectionHandler NewConnection;
        public event UserAccessEvent UserLoginEvent;
        public event UserAccessEvent UserLogoutEvent;
        public event ErrorEvent ErrorHandler;

        public int numberOfConnections = 0;

        public FtpServer(IAccountManager accountManager) : this(new AzureFileSystemFactory(accountManager))
        {
        }

        public FtpServer() : this(new AzureFileSystemFactory(new DefaultAccountManager()))
        {
        }

        public FtpServer(IFileSystemClassFactory fileSystemClassFactory)
        {
            m_apConnections = new ArrayList();
            m_fileSystemClassFactory = fileSystemClassFactory;
        }

        public bool Started
        {
            get { return m_started; }
        }

        public void Start()
        {
            try
            {
                // initialise the encoding of the control channel
                InitialiseConnectionEncoding();

                // initialise the max number of clients
                InitialiseMaxClients();

                m_theThread = new Thread(ThreadRun);
                m_theThread.Start();
                m_started = true;
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(e);
            }
        }

        public void Stop()
        {
            for (int nConnection = 0; nConnection < m_apConnections.Count; nConnection++)
            {
                var handler = m_apConnections[nConnection] as FtpSocketHandler;

                handler?.Stop();
            }

            m_socketListen.Stop();
            m_theThread.Join();
            m_started = false;
        }

        /// The main thread of the ftp server
        /// Listen and acception clients, create handler threads for each client
        private void ThreadRun()
        {
            try
            {
                FtpServerMessageHandler.Message += TraceMessage;

                // listen at the port by the "FTP" endpoint setting
                IPEndPoint ipEndPoint;

                ipEndPoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["FTP2Azure.Command"].IPEndpoint;

                m_socketListen = SocketHelpers.CreateTcpListener(ipEndPoint);

                if (m_socketListen != null)
                {
                    Trace.TraceInformation($"FTP Server listened at: {ipEndPoint}");

                    m_socketListen.Start();

                    Trace.TraceInformation("FTP Server Started");

                    bool fContinue = true;

                    while (fContinue)
                    {
                        TcpClient socket = null;

                        try
                        {
                            socket = m_socketListen.AcceptTcpClient();
                        }
                        catch (SocketException e)
                        {
                            fContinue = false;
                            ErrorHandler?.Invoke(e);
                        }
                        finally
                        {
                            if (socket == null)
                            {
                                fContinue = false;
                            }
                            else if (m_apConnections.Count >= m_maxClients)
                            {
                                Trace.WriteLine("Too many clients, won't handle this connection", "Warnning");
                                SendRejectMessage(socket);
                                socket.Close();
                            }
                            else
                            {
                                socket.NoDelay = false;

                                m_nId++;

                                FtpServerMessageHandler.SendMessage(m_nId, "New connection");

                                SendAcceptMessage(socket);

                                //under stress testing, this happens. Don't know why yet, but let's keep it from crashing
                                try
                                {
                                    InitialiseSocketHandler(socket);
                                }
                                catch (ObjectDisposedException ode)
                                {
                                    Trace.TraceError($"ObjectDisposedException initializing client socket:\r\n{ode}");

                                    m_nId--;
                                    socket.Close();
                                }
                            }
                        }
                    }
                }
                else
                {
                    FtpServerMessageHandler.SendMessage(0, "Error in starting FTP server");
                }
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(e);
            }
        }

        /// Init the encoding of the control channel by the Role setting "ConnectionEncoding"
        /// If the value is "ASCII", encoding = Encoding.ASCII
        /// Otherwise, m_encoding = Encoding.UTF8
        private void InitialiseConnectionEncoding()
        {
            string encoding = RoleEnvironment.GetConfigurationSettingValue("FTP2Azure.ConnectionEncoding");
            switch (encoding)
            {
                case "ASCII":
                    m_encoding = Encoding.ASCII;
                    Trace.WriteLine("Set ftp connection encoding: ASCII", "Information");
                    break;
                case "UTF8":
                default:
                    m_encoding = Encoding.UTF8;
                    Trace.WriteLine("Set ftp connection encoding: UTF8", "Information");
                    break;
            }
        }

        /// Init the member variable m_maxClient by the Role setting "MaxClients"
        /// If any exception or error happens, use default value 5
        private void InitialiseMaxClients()
        {
            string maxClients = RoleEnvironment.GetConfigurationSettingValue("FTP2Azure.MaxClients");

            int iMaxClients = 5;

            try
            {
                iMaxClients = Convert.ToInt32(maxClients);
            }
            catch (Exception)
            {
                // if the "MaxClients" setting is invalid to convert into integer, use default value
                Trace.WriteLine($"Invalid MaxClients setting: {maxClients}", "Warnning");
            }

            if (iMaxClients <= 0)
            {
                Trace.WriteLine($"Invalid MaxClients setting: {maxClients}", "Warnning");
                // negtive or 0 is also invalid, use default value
                iMaxClients = 5;
            }

            m_maxClients = iMaxClients;
        }

        private void SendAcceptMessage(TcpClient socket)
        {
            SocketHelpers.Send(socket, m_encoding.GetBytes("220 FTP to Windows Azure Blob Storage Bridge Ready\r\n"));
        }

        private void SendRejectMessage(TcpClient socket)
        {
            SocketHelpers.Send(socket, m_encoding.GetBytes("421 Too many users now\r\n"));
        }

        private void InitialiseSocketHandler(TcpClient socket)
        {
            var handler = new FtpSocketHandler(m_fileSystemClassFactory, m_nId);
            handler.UserLoginEvent += UserLoginEvent;
            handler.UserLogoutEvent += UserLogoutEvent;
            // get encoding for the socket connection
            handler.Start(socket, m_encoding);

            m_apConnections.Add(handler);

            numberOfConnections = m_apConnections.Count;

            Trace.WriteLine($"Add a new handler, current connection number is {numberOfConnections}", "Information");

            handler.Closed += Handler_Closed;

            NewConnection?.Invoke(m_nId);
        }

        private void Handler_Closed(FtpSocketHandler handler)
        {
            m_apConnections.Remove(handler);

            numberOfConnections = m_apConnections.Count;

            Trace.WriteLine($"Remover a handler, current connection number is {numberOfConnections}", "Information");

            ConnectionClosed?.Invoke(handler.Id);
        }

        public void TraceMessage(int nId, string sMessage)
        {
            Trace.WriteLine($"{nId}: {sMessage}", "FtpServerMessage");
        }

        public void Dispose()
        {
            m_socketListen?.Stop();
        }
    }
}
