using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// ABOR command handler
    /// abort current data connection, TODO
    /// </summary>
    internal class AbortCommandHandler : FtpCommandHandler
    {
        public AbortCommandHandler(FtpConnectionObject connectionObject)
            : base("ABOR", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            // TODO: stop current service & close data connection
            return GetMessage(226, "Current data connection aborted");
        }
    }
}