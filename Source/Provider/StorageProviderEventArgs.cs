using System;

namespace AzureFtpServer.Provider
{
    public class StorageProviderEventArgs : EventArgs
    {
        public StorageOperation Operation;
        public StorageOperationResult Result;
    }
}