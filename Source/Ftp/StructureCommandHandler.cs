using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// STRU command handler
    /// show the data structure of this connection
    /// </summary>
    internal class StructureCommandHandler : FtpCommandHandler
    {
        public StructureCommandHandler(FtpConnectionObject connectionObject)
            : base("STRU", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            switch (sMessage.ToUpper())
            {
                case "F":
                    ConnectionObject.DataStructure = DataStructure.File;
                    return GetMessage(200, string.Format("{0} command succeeded, Structure is File", Command));
                case "R":
                case "P":
                    ConnectionObject.DataStructure = DataStructure.File;
                    return GetMessage(504, string.Format("Data structure {0} is not supported, use File Structure", sMessage));
                default:
                    return GetMessage(501, string.Format("Error - Unknown data structure \"{0}\"", sMessage));
            }
        }
    }
}