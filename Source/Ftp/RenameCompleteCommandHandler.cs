using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.General;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// RNTO command handler
    /// </summary>
    internal class RenameCompleteCommandHandler : FtpCommandHandler
    {
        public RenameCompleteCommandHandler(FtpConnectionObject connectionObject)
            : base("RNTO", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            if (ConnectionObject.FileToRename.Length == 0)
            {
                return GetMessage(503, "RNTO must be preceded by a RNFR.");
            }

            string sOldFileName = ConnectionObject.FileToRename;
            ConnectionObject.FileToRename = ""; // note: 

            sMessage = sMessage.Trim();
            string sNewFileName = GetPath(sMessage);
            // check whether the new filename is valid
            if (!FileNameHelpers.IsValid(sNewFileName) || sNewFileName.EndsWith(@"/"))
            {
                return GetMessage(553, string.Format("\"{0}\" is not a valid file name", sMessage));
            }
            
            // check whether the new filename exists
            // note: azure allows file&virtualdir has the same name
            if (ConnectionObject.FileSystemObject.FileExists(sNewFileName))
            {
                return GetMessage(553, string.Format("File already exists ({0}).", sMessage));
            }

            if (!ConnectionObject.FileSystemObject.Move(sOldFileName, sNewFileName))
            {
                return GetMessage(553, "Rename failed");
            }

            return GetMessage(250, "Renamed file successfully.");
        }
    }
}