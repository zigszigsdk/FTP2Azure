using System;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace AzureFtpServer.Provider
{
    public class BlobRequestOptionsFactory
    {
        public static BlobRequestOptions CreateDefault()
        {
            return new BlobRequestOptions
            {
                MaximumExecutionTime = TimeSpan.FromMinutes(30),
                ServerTimeout = TimeSpan.FromMinutes(30),
                RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(10), 3),
            };
        }
    }
}