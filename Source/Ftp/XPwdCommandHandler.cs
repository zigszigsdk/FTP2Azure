using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// XPWD command handler
    /// Present working directory
    /// </summary>
    internal class XPwdCommandHandler : PwdCommandHandlerBase
    {
        public XPwdCommandHandler(FtpConnectionObject connectionObject)
            : base("XPWD", connectionObject)
        {
        }
    }
}