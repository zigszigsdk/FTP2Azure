using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// PWD command handler
    /// Present working directory
    /// </summary>
    internal class PwdCommandHandler : PwdCommandHandlerBase
    {
        public PwdCommandHandler(FtpConnectionObject connectionObject)
            : base("PWD", connectionObject)
        {
        }
    }
}