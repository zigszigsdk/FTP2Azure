using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.FileSystem;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// MDTM command handler
    /// show last modified time of files
    /// </summary>
    internal class MdtmCommandHandler : FtpCommandHandler
    {
        public MdtmCommandHandler(FtpConnectionObject connectionObject)
            : base("MDTM", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            string sPath = GetPath(sMessage.Trim());

            if (!ConnectionObject.FileSystemObject.FileExists(sPath))
            {
                return GetMessage(550, string.Format("File doesn't exist ({0})", sPath));
            }

            IFileInfo info = ConnectionObject.FileSystemObject.GetFileInfo(sPath);

            if (info == null)
            {
                return GetMessage(550, "Error in getting file information");
            }

            return GetMessage(213, info.GetModifiedTime().ToString());
        }
    }
}
