using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// ALLO command hander
    /// allocate space for files, no need in this ftp server
    /// </summary>
    internal class AlloCommandHandler : FtpCommandHandler
    {
        public AlloCommandHandler(FtpConnectionObject connectionObject)
            : base("ALLO", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            return GetMessage(202, "Do not require to declare the maximum size of the file beforehand");
        }
    }
}