using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.General;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// base class for MKD & XMKD command handlers
    /// </summary>
    internal class MakeDirectoryCommandHandlerBase : FtpCommandHandler
    {
        protected MakeDirectoryCommandHandlerBase(string sCommand, FtpConnectionObject connectionObject)
            : base(sCommand, connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();
            if (sMessage == "")
                return GetMessage(500, string.Format("{0} needs a paramter", Command));

            string dirToMake = GetPath(FileNameHelpers.AppendDirTag(sMessage));

            // check directory name
            if (!FileNameHelpers.IsValid(dirToMake))
                return GetMessage(553, string.Format("\"{0}\": Invalid directory name", sMessage));

            // check whether directory already exists
            if (ConnectionObject.FileSystemObject.DirectoryExists(dirToMake))
                return GetMessage(553, string.Format("Directory \"{0}\" already exists", sMessage));

            // create directory
            if (!ConnectionObject.FileSystemObject.CreateDirectory(dirToMake))
            {
                return GetMessage(550, string.Format("Couldn't create directory. ({0})", sMessage));
            }

            return GetMessage(257, string.Format("{0} successful \"{1}\".", Command, dirToMake));
        }
    }
}