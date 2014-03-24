using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// XCUP command handler
    /// change to the parent directory
    /// </summary>
    internal class XCdupCommandHandler : CdupCommandHandlerBase
    {
        public XCdupCommandHandler(FtpConnectionObject connectionObject)
            : base("XCUP", connectionObject)
        {
        }
    }
}