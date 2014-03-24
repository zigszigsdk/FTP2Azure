using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.General;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// RETR command handler
    /// retreive file from ftp server
    /// </summary>
    internal class RetrCommandHandler : FtpCommandHandler
    {
        private const int m_nBufferSize = 1048576;

        public RetrCommandHandler(FtpConnectionObject connectionObject)
            : base("RETR", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();
            if (sMessage == "")
                return GetMessage(501, string.Format("{0} needs a parameter", Command));
            
            string sFilePath = GetPath(sMessage);

            if (!ConnectionObject.FileSystemObject.FileExists(sFilePath))
            {
                return GetMessage(550, string.Format("File \"{0}\" doesn't exist", sMessage));
            }

            var socketData = new FtpDataSocket(ConnectionObject);
            
            if (!socketData.Loaded)
            {
                return GetMessage(425, "Unable to establish the data connection");
            }

            SocketHelpers.Send(ConnectionObject.Socket, "150 Starting data transfer, please wait...\r\n", ConnectionObject.Encoding);
            
            IFile file = ConnectionObject.FileSystemObject.OpenFile(sFilePath, false);

            if (file == null)
            {
                return GetMessage(550, "Couldn't open file");
            }

            // TYPE I, default
            if (ConnectionObject.DataType == DataType.Image)
            {
                var abBuffer = new byte[m_nBufferSize];

                int nRead = file.Read(abBuffer, m_nBufferSize);

                while (nRead > 0 && socketData.Send(abBuffer, nRead))
                {
                    nRead = file.Read(abBuffer, m_nBufferSize);
                }
            }
            // TYPE A
            else if (ConnectionObject.DataType == DataType.Ascii)
            {
                int writeSize = SocketHelpers.CopyStreamAscii(file.stream, socketData.Socket.GetStream(), m_nBufferSize);
                FtpServerMessageHandler.SendMessage(ConnectionObject.Id, string.Format("Use ascii type success, write {0} chars!", writeSize));
            }
            else // mustn't reach
            {
                file.Close();
                socketData.Close();
                return GetMessage(451, "Error in transfer data: invalid data type.");
            }

            file.Close();
            socketData.Close();

            return GetMessage(226, "File download succeeded.");
        }
    }
}