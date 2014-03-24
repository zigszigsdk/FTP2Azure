using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// MKD command handler
    /// make directory
    /// </summary>
    internal class MakeDirectoryCommandHandler : MakeDirectoryCommandHandlerBase
    {
        public MakeDirectoryCommandHandler(FtpConnectionObject connectionObject)
            : base("MKD", connectionObject)
        {
        }
    }
}