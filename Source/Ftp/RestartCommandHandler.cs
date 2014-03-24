using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// REST command handler
    /// </summary>
    internal class RestartCommandHandler : FtpCommandHandler
    {
        public RestartCommandHandler(FtpConnectionObject connectionObject)
            : base("REST", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            return GetMessage(500, "Restart not supported!");
        }
    }
}