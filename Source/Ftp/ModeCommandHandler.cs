using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// MODE command handler
    /// show the transmission mode of this connection
    /// </summary>
    internal class ModeCommandHandler : FtpCommandHandler
    {
        public ModeCommandHandler(FtpConnectionObject connectionObject)
            : base("MODE", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            switch (sMessage.ToUpper())
            {
                case "S":
                    ConnectionObject.TransmissionMode = TransmissionMode.Stream;
                    return GetMessage(200, string.Format("{0} command succeeded, transmission mode is Stream", Command));
                case "B":
                case "C":
                    ConnectionObject.TransmissionMode = TransmissionMode.Stream;
                    return GetMessage(504, string.Format("Transfer mode {0} is not supported, use Stream Mode", sMessage));
                default:
                    return GetMessage(501, string.Format("Error - Unknown transimmsion mode \"{0}\"", sMessage));
            }
        }
    }
}