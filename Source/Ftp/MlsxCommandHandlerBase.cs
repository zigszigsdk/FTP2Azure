using System.Text;
using AzureFtpServer.Ftp;
using AzureFtpServer.Ftp.General;
using AzureFtpServer.General;
using AzureFtpServer.Ftp.FileSystem;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// Base class for MLST & MLSD command handlers
    /// </summary>
    internal class MlsxCommandHandlerBase : FtpCommandHandler
    {
        protected MlsxCommandHandlerBase(string sCommand, FtpConnectionObject connectionObject)
            : base(sCommand, connectionObject)
        {
        }

        protected string GenerateEntry(IFileInfo info)
        {
            StringBuilder entry = new StringBuilder();

            bool isDirectory = info.IsDirectory();

            if (isDirectory)
            {
                entry.Append("Type=dir; ");
                string dirName = FileNameHelpers.GetDirectoryName(info.Path());
                entry.Append(dirName);
            }
            else
            {
                entry.Append(string.Format("Type=file;Size={0};Modify={1}; ", info.GetSize(), info.GetModifiedTime().ToString("yyyyMMddHHmmss")));
                entry.Append(FileNameHelpers.GetFileName(info.Path()));
            }

            return entry.ToString();
        }
        
    }
}