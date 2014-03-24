using System.IO;
using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.General;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// base class for CWD & XCWD command handlers
    /// </summary>
    internal class CwdCommandHandlerBase : FtpCommandHandler
    {
        public CwdCommandHandlerBase(string sCommand, FtpConnectionObject connectionObject)
            : base(sCommand, connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();
            if (sMessage.Length == 0)
                return GetMessage(501, string.Format("Syntax error. {0} needs a parameter", Command));

            // append the final '/' char
            string sMessageFull = FileNameHelpers.AppendDirTag(sMessage);

            #region change to the parent dir
            if (sMessageFull == @"../")
            {
                // get the parent directory
                string parentDir = GetParentDir();
                if (parentDir == null)
                    return GetMessage(550, "Root directory, cannot change to the parent directory");

                ConnectionObject.CurrentDirectory = parentDir;
                return GetMessage(200, string.Format("{0} Successful ({1})", Command, parentDir));
            }
            #endregion
            
            if (!FileNameHelpers.IsValid(sMessageFull))
            {
                return GetMessage(550, string.Format("\"{0}\" is not a valid directory string.", sMessage));
            }

            // get the new directory path
            string newDirectory = GetPath(sMessageFull);

            // checks whether the new directory exists
            if (!ConnectionObject.FileSystemObject.DirectoryExists(newDirectory))
            {
                return GetMessage(550, string.Format("\"{0}\" no such directory.", sMessage));
            }

            ConnectionObject.CurrentDirectory = newDirectory;
            return GetMessage(250, string.Format("{0} Successful ({1})", Command, newDirectory));
        }
    }
}