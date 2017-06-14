using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.Ftp;

namespace AzureFtpServer.Azure
{
    public class AzureFileSystemFactory : IFileSystemClassFactory
    {
        private readonly IAccountManager m_accountManager;

        public AzureFileSystemFactory(IAccountManager accountManager)
        {
            m_accountManager = accountManager;
            m_accountManager.LoadConfigration();
        }

        public IFileSystem Create(string sUser, string sPassword)
        {
            if (string.IsNullOrWhiteSpace(sUser) || string.IsNullOrWhiteSpace(sPassword))
                return null;

            if (!m_accountManager.CheckAccount(sUser, sPassword))
                return null;
            
            string containerName = sUser; 
            var system = new AzureFileSystem(containerName);

            return system;
        }
    }
}