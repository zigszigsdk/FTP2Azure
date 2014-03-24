using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// PASS command handler
    /// get password and do login
    /// </summary>
    internal class PasswordCommandHandler : FtpCommandHandler
    {
        public PasswordCommandHandler(FtpConnectionObject connectionObject)
            : base("PASS", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            sMessage = sMessage.Trim();
            if (sMessage == "")
                return GetMessage(501, string.Format("{0} needs a parameter", Command));

            if (ConnectionObject.Login(sMessage))
            {
                return GetMessage(220, "Password ok, FTP server ready");
            }
            else
            {
                return GetMessage(530, "Username or password incorrect");
            }
        }
    }
}