using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// PORT command handler
    /// enter active mode
    /// </summary>
    internal class PortCommandHandler : FtpCommandHandler
    {
        public PortCommandHandler(FtpConnectionObject connectionObject)
            : base("PORT", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            string[] asData = sMessage.Split(new[] {','});

            if (asData.Length != 6)
            {
                return GetMessage(501, string.Format("{0}: Syntax error in parameters", Command));
            }

            ConnectionObject.DataConnectionType = DataConnectionType.Active;

            int nSocketPort = int.Parse(asData[4])*256 + int.Parse(asData[5]);

            ConnectionObject.PortCommandSocketPort = nSocketPort;
            ConnectionObject.PortCommandSocketAddress = string.Join(".", asData, 0, 4);

            return GetMessage(200, string.Format("{0} successful.", Command));
        }
    }
}