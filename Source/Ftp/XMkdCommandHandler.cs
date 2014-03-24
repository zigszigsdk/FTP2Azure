using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// XMKD command handler
    /// make directory
    /// </summary>
    internal class XMkdCommandHandler : MakeDirectoryCommandHandlerBase
    {
        public XMkdCommandHandler(FtpConnectionObject connectionObject)
            : base("XMKD", connectionObject)
        {
        }
    }
}