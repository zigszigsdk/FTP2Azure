using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// XCWD command handler
    /// change working directory
    /// </summary>
    internal class XCwdCommandHandler : CwdCommandHandlerBase
    {
        public XCwdCommandHandler(FtpConnectionObject connectionObject)
            : base("XCWD", connectionObject)
        {
        }
    }
}