using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// CWD command handler
    /// change working directory
    /// </summary>
    internal class CwdCommandHandler : CwdCommandHandlerBase
    {
        public CwdCommandHandler(FtpConnectionObject connectionObject)
            : base("CWD", connectionObject)
        {
        }
    }
}