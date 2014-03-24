using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// RMD command handler
    /// remove directory
    /// </summary>
    internal class RemoveDirectoryCommandHandler : RemoveDirectoryCommandHandlerBase
    {
        public RemoveDirectoryCommandHandler(FtpConnectionObject connectionObject)
            : base("RMD", connectionObject)
        {
        }
    }
}