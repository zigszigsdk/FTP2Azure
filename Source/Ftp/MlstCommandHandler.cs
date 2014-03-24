using System.Text;
using AzureFtpServer.General;
using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.General;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// MLST command handler
    /// only list file/directory info
    /// </summary>
    class MlstCommandHandler : MlsxCommandHandlerBase
    {
        public MlstCommandHandler(FtpConnectionObject connectionObject)
            : base("MLST", connectionObject)
        { 
        }

        protected override string OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();

            // Get the file/dir to list
            string targetToList = GetPath(sMessage);

            // checks the file/dir name
            if (!FileNameHelpers.IsValid(targetToList))
            {
                return GetMessage(501, string.Format("\"{0}\" is not a valid file/directory name", sMessage));
            }

            bool targetIsFile = ConnectionObject.FileSystemObject.FileExists(targetToList);
            bool targetIsDir = ConnectionObject.FileSystemObject.DirectoryExists(FileNameHelpers.AppendDirTag(targetToList));

            if (!targetIsFile && !targetIsDir)
                return GetMessage(550, string.Format("\"{0}\" not exists", sMessage));

            SocketHelpers.Send(ConnectionObject.Socket, string.Format("250- MLST {0}\r\n", targetToList), ConnectionObject.Encoding);

            StringBuilder response = new StringBuilder();

            if (targetIsFile)
            {
                response.Append(" ");
                var fileInfo = ConnectionObject.FileSystemObject.GetFileInfo(targetToList);
                response.Append(GenerateEntry(fileInfo));
                response.Append("\r\n");
            }
            
            if (targetIsDir)
            {
                response.Append(" ");
                var dirInfo = ConnectionObject.FileSystemObject.GetDirectoryInfo(FileNameHelpers.AppendDirTag(targetToList));
                response.Append(GenerateEntry(dirInfo));
                response.Append("\r\n");
            }

            SocketHelpers.Send(ConnectionObject.Socket, response.ToString(), ConnectionObject.Encoding);

            return GetMessage(250, "MLST successful");
        }
    }
}
