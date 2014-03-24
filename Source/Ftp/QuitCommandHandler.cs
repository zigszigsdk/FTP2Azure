using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// QUIT command handler
    /// </summary>
    internal class QuitCommandHandler : FtpCommandHandler
    {
        public QuitCommandHandler(FtpConnectionObject connectionObject)
            : base("QUIT", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            return GetMessage(220, "Goodbye");
        }
    }
}