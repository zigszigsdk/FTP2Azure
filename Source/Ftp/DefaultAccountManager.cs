using System.Diagnostics;
using System.Collections.Generic;
using AzureFtpServer.Provider;

namespace AzureFtpServer.Ftp
{
    /// AccountManager Class
    /// Read account information from config settings and store valid (username,password) pairs
    public class DefaultAccountManager : IAccountManager
    {
        private const char separator = ':';
        private Dictionary<string, string> _accounts;
        private int _usernum;

        public DefaultAccountManager()
        {
            _accounts = new Dictionary<string, string>();
            _usernum = 0;
        }

        
        public int UserNum 
        {
            get { return _usernum; }
        }

        /// Read the settings in RoleEnvironment, insert into the accounts dictionary
        public int LoadConfigration()
        {
            // init member vars 
            _usernum = 0;
            _accounts.Clear();

            //int idx = 1; // use idx to concat the setting name
            
            //bool hasNext = true; // bool value, need find the next setting
            // Get the account setting
            string accountInfo = StorageProviderConfiguration.FtpAccount;

            while (true)
            {
                // Get the begin tag (
                int beginIdx = accountInfo.IndexOf('(');
                // Get the end tag )
                int endIdx = accountInfo.IndexOf(')');

                if ((beginIdx < 0) || (endIdx < 0) || (beginIdx > endIdx) || (endIdx == beginIdx + 1))
                    break;

                string oneAccount = accountInfo.Substring(beginIdx + 1, endIdx - beginIdx - 1);

                // modify accountInfo for loop
                accountInfo = accountInfo.Substring(endIdx + 1);

                int separatoridx = oneAccount.IndexOf(separator);

                if (separatoridx < 0)
                {
                    Trace.WriteLine(string.Format("Invalid <username, password> pair ({0}) in cscfg.", oneAccount), "Warnning");
                    continue;
                }

                // get the username substr
                string username = oneAccount.Substring(0, separatoridx);

                // check the username whether conform to the naming rules
                if (!AccountValidator.IsValid(username))
                {
                    Trace.WriteLine(string.Format("Invalid <username, password> pair ({0}) in cscfg.", oneAccount), "Warnning");
                    continue;
                }

                // check whether the username already exists
                if (_accounts.ContainsKey(username))
                    continue;

                // get the password substr
                string password = oneAccount.Substring(separatoridx + 1);
                // simple check, password can not be empty
                if (password.Length == 0)
                    continue;

                _accounts.Add(username, password);
                _usernum++;
            }

            Trace.WriteLine(string.Format("Load {0} accounts.", _usernum), "Information");
            
            return _usernum;
        }

        public bool CheckAccount(string username, string password)
        {
            if (!_accounts.ContainsKey(username))
                return false;
            return (_accounts[username] == password);
        }

        public string GetPassword(string username) 
        {
            if (username == null || !_accounts.ContainsKey(username))
                return null; 
            return _accounts[username];
           
        }
    }
}
