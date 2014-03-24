using System;
using System.Text;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.Provider;

namespace AzureFtpServer.Azure
{
    public sealed class AzureFileInfo : IFileInfo
    {
        private readonly AzureCloudFile _file;

        public AzureFileInfo(AzureCloudFile file)
        {
            _file = file;
        }
        
        #region Implementation of IFileInfo 

        public bool FileObjectExists()
        {
            return _file != null;
        }

        public string Path()
        {
            return _file.FtpPath;
        }

        public DateTimeOffset GetModifiedTime()
        {
            return _file.LastModified;
        }

        public long GetSize()
        {
            return _file.Size;
        }

        public bool IsDirectory()
        {
            return _file.IsDirectory;
        }

        public string GetAttributeString()
        {
            bool fDirectory = IsDirectory();
            bool fReadOnly = false; // No file should be read-only.

            var builder = new StringBuilder();

            builder.Append(fDirectory ? "d" : "-");

            builder.Append("r");

            if (fReadOnly)
            {
                builder.Append("-");
            }
            else
            {
                builder.Append("w");
            }

            builder.Append(fDirectory ? "x" : "-");

            builder.Append(fDirectory ? "r-xr-x" : "r--r--");

            return builder.ToString();
        }

        #endregion
    }
}