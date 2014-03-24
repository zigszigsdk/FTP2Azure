using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// SMNT command handler, superfluous at this site
    /// </summary>
    internal class SmntCommandHandler : FtpCommandHandler
    {
        public SmntCommandHandler(FtpConnectionObject connectionObject)
            : base("SMNT", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            return GetMessage(202, "");
        }
    }
}