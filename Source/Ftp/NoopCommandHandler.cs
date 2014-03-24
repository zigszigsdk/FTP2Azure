using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// NOOP command handler
    /// </summary>
    internal class NoopCommandHandler : FtpCommandHandler
    {
        public NoopCommandHandler(FtpConnectionObject connectionObject)
            : base("NOOP", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            return GetMessage(200, "");
        }
    }
}