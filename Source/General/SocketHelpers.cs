using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography;
using AzureFtpServer.Provider;

namespace AzureFtpServer.General
{
    public sealed class SocketHelpers
    {
        private SocketHelpers()
        {
        }

        public static bool Send(TcpClient socket, byte[] abMessage)
        {
            return Send(socket, abMessage, 0, abMessage.Length);
        }

        public static bool Send(TcpClient socket, byte[] abMessage, int nStart)
        {
            return Send(socket, abMessage, nStart, abMessage.Length - nStart);
        }

        public static bool Send(TcpClient socket, byte[] abMessage, int nStart, int nLength)
        {
            bool fReturn = true;

            try
            {
                var writer = new BinaryWriter(socket.GetStream());
                writer.Write(abMessage, nStart, nLength);
                writer.Flush();
            }
            catch (IOException)
            {
                fReturn = false;
            }
            catch (SocketException)
            {
                fReturn = false;
            }

            return fReturn;
        }

        public static bool Send(TcpClient socket, string sMessage, Encoding encoding)
        {
            byte[] abMessage = encoding.GetBytes(sMessage);
            return Send(socket, abMessage);
        }

        public static void Close(TcpClient socket)
        {
            if (socket == null)
            {
                return;
            }

            try
            {
                if (socket.GetStream() != null)
                {
                    try
                    {
                        socket.GetStream().Flush();
                    }
                    catch (SocketException)
                    {
                    }

                    try
                    {
                        socket.GetStream().Close();
                    }
                    catch (SocketException)
                    {
                    }
                }
            }
            catch (SocketException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                socket.Close();
            }
            catch (SocketException)
            {
            }
        }

        public static TcpClient CreateTcpClient(string sAddress, int nPort)
        {
            TcpClient client = null;

            try
            {
                client = new TcpClient(sAddress, nPort);
            }
            catch (SocketException)
            {
                client = null;
            }

            return client;
        }

        public static TcpListener CreateTcpListener(IPEndPoint endPoint)
        {
            TcpListener listener = null;

            try
            {
                listener = new TcpListener(endPoint);
            }
            catch (SocketException)
            {
                listener = null;
            }

            return listener;
        }

        public static IPAddress GetLocalAddress()
        {
            string ftpHost = null;
            if (StorageProviderConfiguration.Mode == Modes.Live)
                ftpHost = StorageProviderConfiguration.FtpServerHost;
            else
                ftpHost = "localhost";
            IPHostEntry hostEntry = Dns.GetHostEntry(ftpHost);
            IPAddress retAddress = null;
            foreach (var ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    retAddress = ip;
            }

            return retAddress;
        }

        public static int CopyStreamAscii(Stream input, Stream output, int bufferSize)
        {
            char[] buffer = new char[bufferSize];
            int count = 0;
            int total = 0;
            using (StreamReader rdr = new StreamReader(input, Encoding.ASCII))
            {
                using (StreamWriter wtr = new StreamWriter(output, Encoding.ASCII))
                {
                    while ((count = rdr.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        wtr.Write(buffer, 0, count);
                        total += count;
                    }
                }
            }
            return total;
        }
    }
}