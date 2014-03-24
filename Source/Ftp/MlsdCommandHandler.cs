using System.Text;
using AzureFtpServer.General;
using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.General;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// MLSD command handler
    /// only list content under directories
    /// </summary>
    class MlsdCommandHandler : MlsxCommandHandlerBase
    {
        public MlsdCommandHandler(FtpConnectionObject connectionObject)
            : base("MLSD", connectionObject)
        { 
        }

        protected override string OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();

            // Get the dir to list
            string targetToList = GetPath(sMessage);

            // checks the dir name
            if (!FileNameHelpers.IsValid(targetToList))
            {
                return GetMessage(501, string.Format("\"{0}\" is not a valid directory name", sMessage));
            }

            // specify the directory tag
            targetToList = FileNameHelpers.AppendDirTag(targetToList);

            bool targetIsDir = ConnectionObject.FileSystemObject.DirectoryExists(targetToList);

            if (!targetIsDir)
                return GetMessage(550, string.Format("Directory \"{0}\" not exists", targetToList));

            #region Generate response

            StringBuilder response = new StringBuilder();

            string[] files = ConnectionObject.FileSystemObject.GetFiles(targetToList);
            string[] directories = ConnectionObject.FileSystemObject.GetDirectories(targetToList);

            if (files != null)
            {
                foreach (var file in files)
                { 
                    var fileInfo = ConnectionObject.FileSystemObject.GetFileInfo(file);
                    
                    response.Append(GenerateEntry(fileInfo));
                    
                    response.Append("\r\n");
                }
            }

            if (directories != null)
            {
                foreach (var dir in directories)
                {
                    var dirInfo = ConnectionObject.FileSystemObject.GetDirectoryInfo(dir);

                    response.Append(GenerateEntry(dirInfo));

                    response.Append("\r\n");
                }
            }

            #endregion

            #region Write response

            var socketData = new FtpDataSocket(ConnectionObject);

            if (!socketData.Loaded)
            {
                return GetMessage(425, "Unable to establish the data connection");
            }

            SocketHelpers.Send(ConnectionObject.Socket, "150 Opening data connection for MLSD\r\n", ConnectionObject.Encoding);

            // ToDo, send response according to ConnectionObject.DataType, i.e., Ascii or Binary
            socketData.Send(response.ToString(), Encoding.UTF8);
            socketData.Close();

            #endregion

            return GetMessage(226, "MLSD successful");
        }
    }
}
