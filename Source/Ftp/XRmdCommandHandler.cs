using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// XRMD command handler
    /// remove directory
    /// </summary>
    internal class XRmdCommandHandler : RemoveDirectoryCommandHandlerBase
    {
        public XRmdCommandHandler(FtpConnectionObject connectionObject)
            : base("XRMD", connectionObject)
        {
        }
    }
}