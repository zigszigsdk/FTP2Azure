using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// DELE command handler
    /// delete a file
    /// </summary>
    internal class DeleCommandHandler : FtpCommandHandler
    {
        public DeleCommandHandler(FtpConnectionObject connectionObject)
            : base("DELE", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();
            if (sMessage == "")
                return GetMessage(501, string.Format("{0} needs a parameter", Command));

            string fileToDelete = GetPath(sMessage);

            if (!ConnectionObject.FileSystemObject.FileExists(fileToDelete))
            {
                return GetMessage(550, string.Format("File \"{0}\" does not exist.", fileToDelete));
            }

            if (!ConnectionObject.FileSystemObject.DeleteFile(fileToDelete))
            {
                return GetMessage(550, string.Format("Delete file \"{0}\" failed.", fileToDelete));
            }

            return GetMessage(250, string.Format("{0} successful.", Command));
        }
    }
}