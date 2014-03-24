using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.General;
using AzureFtpServer.Provider;

namespace AzureFtpServer.Ftp
{
    /// Contains the socket read functionality. Works on its own thread since all socket operation is blocking.
    internal class FtpSocketHandler
    {

        private const int m_nBufferSize = 65536;
        private readonly IFileSystemClassFactory m_fileSystemClassFactory;
        private readonly int m_nId;
        
        private TcpClient m_theSocket;
        private Thread m_theThread;
        private Thread m_theMonitorThread;
        private static DateTime m_lastActiveTime; // shared between threads
        private int m_maxIdleSeconds;

        public delegate void CloseHandler(FtpSocketHandler handler);

        public event CloseHandler Closed;

        public FtpConnectionObject m_theCommands;


        public event AzureFtpServer.Ftp.FtpServer.UserAccessEvent UserLoginEvent;
        public event AzureFtpServer.Ftp.FtpServer.UserAccessEvent UserLogoutEvent;

        public FtpSocketHandler(IFileSystemClassFactory fileSystemClassFactory, int nId)
        {
            m_nId = nId;
            m_fileSystemClassFactory = fileSystemClassFactory;
        }


        public void Start(TcpClient socket, System.Text.Encoding encoding)
        {
            m_theSocket = socket;

            m_maxIdleSeconds = StorageProviderConfiguration.MaxIdleSeconds;

            m_lastActiveTime = DateTime.Now;

            m_theCommands = new FtpConnectionObject(m_fileSystemClassFactory, m_nId, socket);
            m_theCommands.Encoding = encoding;
            m_theCommands.UserLoginEvent += UserLoginEvent;
            m_theCommands.UserLogoutEvent += UserLogoutEvent;
            m_theThread = new Thread(ThreadRun);
            m_theThread.Start();
            m_theMonitorThread = new Thread(ThreadMonitor);
            m_theMonitorThread.Start();
        }

        public void Stop()
        {
            SocketHelpers.Close(m_theSocket);
            m_theThread.Join();
            m_theMonitorThread.Join();
        }

        private void ThreadRun()
        {
            var abData = new Byte[m_nBufferSize];

            try
            {
                int nReceived = m_theSocket.GetStream().Read(abData, 0, m_nBufferSize);

                while (nReceived > 0)
                {
                    m_theCommands.Process(abData);

                    // the Read method will block
                    nReceived = m_theSocket.GetStream().Read(abData, 0, m_nBufferSize);

                    m_lastActiveTime = DateTime.Now;
                }
            }
            catch (SocketException)
            {
            }
            catch (IOException)
            {
            }

            FtpServerMessageHandler.SendMessage(m_nId, "Connection closed");

            if (Closed != null)
            {
                Closed(this);
            }


            m_theCommands.LogOut();

            m_theSocket.Close();
        }

        private void ThreadMonitor()
        {
            while (m_theThread.IsAlive)
            {
                DateTime currentTime = DateTime.Now;
                TimeSpan timeSpan = currentTime - m_lastActiveTime;
                // has been idle for a long time
                if ((timeSpan.TotalSeconds > m_maxIdleSeconds) && !m_theCommands.DataSocketOpen) 
                {
                    SocketHelpers.Send(m_theSocket, string.Format("426 No operations for {0}+ seconds. Bye!", m_maxIdleSeconds), m_theCommands.Encoding);
                    FtpServerMessageHandler.SendMessage(m_nId, "Connection closed for too long idle time.");
                    if (Closed != null)
                    {
                        Closed(this);
                    }
                    m_theSocket.Close();
                    this.Stop();
                    return;
                }
                Thread.Sleep(1000 * m_maxIdleSeconds);
            }

            return; // only monitor the work thread
        }

        public int Id
        {
            get { return m_nId; }
        }

    }
}