using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// NLST command handler
    /// name list
    /// </summary>
    internal class NlstCommandHandler : ListCommandHandlerBase
    {
        public NlstCommandHandler(FtpConnectionObject connectionObject)
            : base("NLST", connectionObject)
        {
        }

        protected override string BuildReply(string[] asFiles, string[] asDirectories)
        {
            return BuildShortReply(asFiles, asDirectories);
        }
    }
}