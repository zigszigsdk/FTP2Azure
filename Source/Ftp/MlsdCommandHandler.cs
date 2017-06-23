using System.Linq;
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
                return GetMessage(501, $"\"{sMessage}\" is not a valid directory name");
            }

            // specify the directory tag
            targetToList = FileNameHelpers.AppendDirTag(targetToList);

            bool targetIsDir = ConnectionObject.FileSystemObject.DirectoryExists(targetToList);

            if (!targetIsDir)
            {
                return GetMessage(550, $"Directory \"{targetToList}\" not exists");
            }

            #region Generate response

            StringBuilder response = new StringBuilder();

            string[] files = ConnectionObject.FileSystemObject.GetFiles(targetToList);
            string[] directories = ConnectionObject.FileSystemObject.GetDirectories(targetToList);

            if (files != null && files.Any())
            {
                foreach (var file in files)
                { 
                    var fileInfo = ConnectionObject.FileSystemObject.GetFileInfo(file);
                    
                    response.Append(GenerateEntry(fileInfo));
                    
                    response.Append("\r\n");
                }
            }

            if (directories != null && directories.Any())
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

            SocketHelpers.Send(ConnectionObject.Socket, $"150 {ConnectionObject.DataType} Opening data connection for MLSD {targetToList}\r\n", ConnectionObject.Encoding);

            try
            {
                // ToDo, send response according to ConnectionObject.DataType, i.e., Ascii or Binary
                socketData.Send(response.ToString(), ConnectionObject.Encoding);
            }
            finally
            {
                socketData.Close();
            }

            #endregion

            return GetMessage(226, "MLSD successful");
        }
    }
}
