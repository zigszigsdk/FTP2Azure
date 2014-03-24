using AzureFtpServer.General;
using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.General;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// STAT command handler
    /// if the parameter is empty, return status message of this ftp server
    /// otherwise, work as LIST commmand
    /// </summary>
    internal class StatCommandHandler : ListCommandHandlerBase
    {
        public StatCommandHandler(FtpConnectionObject connectionObject)
            : base("STAT", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();
            
            if (sMessage == "")
            {
                return GetMessage(211, "Server status: OK");
            }

            // if no parameter is given, STAT works as LIST
            // but won't use data connection
            string[] asFiles = null;
            string[] asDirectories = null;

            // Get the file/dir to list
            string targetToList = GetPath(sMessage);

            // checks the file/dir name
            if (!FileNameHelpers.IsValid(targetToList))
            {
                return GetMessage(501, string.Format("\"{0}\" is not a valid file/directory name", sMessage));
            }

            // two vars indicating different list results
            bool targetIsFile = false;
            bool targetIsDir = false;

            // targetToList ends with '/', must be a directory
            if (targetToList.EndsWith(@"/"))
            {
                targetIsFile = false;
                if (ConnectionObject.FileSystemObject.DirectoryExists(targetToList))
                    targetIsDir = true;
            }
            else
            {
                // check whether the target to list is a directory
                if (ConnectionObject.FileSystemObject.DirectoryExists(FileNameHelpers.AppendDirTag(targetToList)))
                    targetIsDir = true;
                // check whether the target to list is a file
                if (ConnectionObject.FileSystemObject.FileExists(targetToList))
                    targetIsFile = true;
            }

            if (targetIsFile)
            {
                asFiles = new string[1] { targetToList };
                if (targetIsDir)
                    asDirectories = new string[1] { FileNameHelpers.AppendDirTag(targetToList) };
            }
            // list a directory
            else if (targetIsDir)
            {
                targetToList = FileNameHelpers.AppendDirTag(targetToList);
                asFiles = ConnectionObject.FileSystemObject.GetFiles(targetToList);
                asDirectories = ConnectionObject.FileSystemObject.GetDirectories(targetToList);
            }
            else
            {
                return GetMessage(550, string.Format("\"{0}\" not exists", sMessage));
            }

            // generate the response
            string sFileList = BuildReply(asFiles, asDirectories);

            SocketHelpers.Send(ConnectionObject.Socket, string.Format("213-Begin STAT \"{0}\":\r\n", sMessage), ConnectionObject.Encoding);

            SocketHelpers.Send(ConnectionObject.Socket, sFileList, ConnectionObject.Encoding);
            
            return GetMessage(213, string.Format("{0} successful.", Command));
        }

        protected override string BuildReply(string[] asFiles, string[] asDirectories)
        {
            return BuildLongReply(asFiles, asDirectories);
        }
    }
}