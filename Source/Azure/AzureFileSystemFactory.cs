using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.Ftp;

namespace AzureFtpServer.Azure
{
    public class AzureFileSystemFactory : IFileSystemClassFactory
    {
        private AccountManager m_accountManager;

        public AzureFileSystemFactory()
        {
            m_accountManager = new AccountManager();
            m_accountManager.LoadConfigration();
        }


        public IFileSystem Create(string sUser, string sPassword)
        {
            if ((sUser == null) || (sPassword == null))
                return null;

            if (!m_accountManager.CheckAccount(sUser, sPassword))
                return null;
            
            string containerName = sUser; 
            var system = new AzureFileSystem(containerName);
            
            return system;
        }

    }
}