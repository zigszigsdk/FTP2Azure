using System.Net.Sockets;
using System.Text;
using AzureFtpServer.General;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AzureFtpServer.Ftp
{
    /// <summary>
    /// Encapsulates the functionality necessary to send data along the reply connection
    /// </summary>
    internal class FtpDataSocket
    {
        private TcpClient m_theSocket;

        public FtpDataSocket(FtpConnectionObject connectionObject)
        {
            m_theSocket = OpenSocket(connectionObject);
            // the data connection type should be set every time it is used
            connectionObject.DataConnectionType = DataConnectionType.Invalid;
        }

        public TcpClient Socket
        { 
            get { return m_theSocket; }
        }

        public bool Loaded
        {
            get { return m_theSocket != null; }
        }

        public void Close()
        {
            if (m_theSocket != null)
                SocketHelpers.Close(m_theSocket);
            
            m_theSocket = null;
        }

        public bool Send(byte[] abData, int nSize)
        {
            return SocketHelpers.Send(m_theSocket, abData, 0, nSize);
        }

        public bool Send(string sMessage, Encoding encoding)
        {
            byte[] abData = encoding.GetBytes(sMessage);
            return Send(abData, abData.Length);
        }

        public int Receive(byte[] abData)
        {
            return m_theSocket.GetStream().Read(abData, 0, abData.Length);
        }

        private static TcpClient OpenSocket(FtpConnectionObject connectionObject)
        {
            switch (connectionObject.DataConnectionType)
            {
                case DataConnectionType.Active:
                    return SocketHelpers.CreateTcpClient(connectionObject.PortCommandSocketAddress,
                                                         connectionObject.PortCommandSocketPort);
                case DataConnectionType.Passive:
                    return connectionObject.PassiveSocket;
                default:
                    FtpServerMessageHandler.SendMessage(connectionObject.Id, "Invalid connection type!");
                    return null;
            }
        }
    }
}