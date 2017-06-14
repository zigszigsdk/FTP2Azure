using System.Text.RegularExpressions;

namespace AzureFtpServer.Ftp
{
    public class AccountValidator
    {
        /// checks whether username conform to the Azure container naming rules
        /// 1. start with a letter or number, and can contain only letters, numbers, and the dash (-) character
        /// 2. Every dash (-) character must be immediately preceded and followed by a letter or number; 
        ///    consecutive dashes are not permitted in container names.
        /// 3. All letters in a container name must be lowercase.
        /// 4. Container names must be from 3 through 63 characters long.
        public static bool IsValid(string username)
        {
            return Regex.IsMatch(username, @"^\$root$|^[a-z0-9]([a-z0-9]|(?<=[a-z0-9])-(?=[a-z0-9])){2,62}$");
        }
    }
}