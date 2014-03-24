using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.FileSystem;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// RNFR command handler
    /// Starts a rename file operation
    /// </summary>
    internal class RenameStartCommandHandler : FtpCommandHandler
    {
        public RenameStartCommandHandler(FtpConnectionObject connectionObject)
            : base("RNFR", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();

            if (sMessage.Length == 0)
                return GetMessage(501, "Syntax error. RNFR needs a parameter");

            string sFile = GetPath(sMessage);

            // check whether file exists
            if (!ConnectionObject.FileSystemObject.FileExists(sFile))
            {
                return GetMessage(550, string.Format("File {0} not exists. Rename directory not supported.", sMessage));
            }

            ConnectionObject.FileToRename = sFile;

            return GetMessage(350, string.Format("Rename file started ({0}).", sFile));
        }
    }
}