using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// LIST command handler
    /// list detailed information about files/directories
    /// </summary>
    internal class ListCommandHandler : ListCommandHandlerBase
    {
        public ListCommandHandler(FtpConnectionObject connectionObject)
            : base("LIST", connectionObject)
        {
        }

        protected override string BuildReply(string[] asFiles, string[] asDirectories)
        {
            return BuildLongReply(asFiles, asDirectories);
        }
    }
}