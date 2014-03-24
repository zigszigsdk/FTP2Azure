using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// SITE command handler
    /// show site info
    /// </summary>
    internal class SiteCommandHandler : FtpCommandHandler
    {
        public SiteCommandHandler(FtpConnectionObject connectionObject)
            : base("SITE", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            return GetMessage(200, "Ftp Server on Windows Azure, supply operations on Azure Blob Storage.");
        }
    }
}