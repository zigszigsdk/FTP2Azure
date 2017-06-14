namespace AzureFtpServer.Ftp
{
    public interface IAccountManager
    {
        int UserNum { get; }

        int LoadConfigration();

        bool CheckAccount(string username, string password);
        string GetPassword(string username);
    }
}