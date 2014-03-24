using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// base class for CDUP & XCDP command handlers
    /// </summary>
    internal class CdupCommandHandlerBase : FtpCommandHandler
    {
        public CdupCommandHandlerBase(string sCommand, FtpConnectionObject connectionObject)
            : base(sCommand, connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            if (sMessage.Length != 0)
            {
                return GetMessage(501, string.Format("Invalid syntax for {0} command", Command));
            }

            // get the parent directory
            string parentDir = GetParentDir();
            if (parentDir == null)
                return GetMessage(550, "Root directory, cannot change to the parent directory");

            ConnectionObject.CurrentDirectory = parentDir;
            return GetMessage(200, string.Format("{0} Successful ({1})", Command, parentDir));
        }
    }
}