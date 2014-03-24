using System;
using System.Collections.Specialized;

namespace AzureFtpServer.Provider
{
    public sealed class AzureCloudFile
    {
        #region AzureCloudFile Members

        public DateTimeOffset LastModified { get; set; }
        public long Size { get; set; }
        public Uri Uri { get; set; } // not used
        public string FtpPath { get; set; }
        public bool IsDirectory { get; set; }

        #endregion
    }
}