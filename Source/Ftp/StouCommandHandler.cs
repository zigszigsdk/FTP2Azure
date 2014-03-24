using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// STOU command handler, superfluous at this site
    /// Store unique
    /// </summary>
    internal class StouCommandHandler : FtpCommandHandler
    {
        public StouCommandHandler(FtpConnectionObject connectionObject)
            : base("STOU", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            return GetMessage(202, "Use STOR instead");
        }
    }
}