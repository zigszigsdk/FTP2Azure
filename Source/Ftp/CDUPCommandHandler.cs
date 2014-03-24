using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// CDUP command handler
    /// change to the parent directory
    /// </summary>
    internal class CdupCommandHandler : CdupCommandHandlerBase
    {
        public CdupCommandHandler(FtpConnectionObject connectionObject)
            : base("CDUP", connectionObject)
        {
        }
    }
}