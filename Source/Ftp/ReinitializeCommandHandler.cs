using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// REIN command handler
    /// waiting for new user, reset all connection parameters
    /// </summary>
    internal class ReinitializeCommandHandler : FtpCommandHandler
    {
        public ReinitializeCommandHandler(FtpConnectionObject connectionObject)
            : base("REIN", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            if (sMessage.Trim() != "")
                return GetMessage(501, "REIN needs no parameters");

            // log out current user
            ConnectionObject.LogOut();

            return GetMessage(220, "Service ready for new user!");
        }
    }
}