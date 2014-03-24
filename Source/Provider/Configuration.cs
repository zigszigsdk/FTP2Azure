using System;
using System.Configuration;
using Microsoft.WindowsAzure.ServiceRuntime;


namespace AzureFtpServer.Provider
{
    public enum Modes
    {
        Live,
        Debug
    }

    public class StorageProviderConfiguration
    {
        
        public static string FtpAccount
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue("FTP2Azure.FtpAccount");
            }
        }

        public static Modes Mode
        {
            get
            {
                return (Modes)Enum.Parse(typeof(Provider.Modes), RoleEnvironment.GetConfigurationSettingValue("FTP2Azure.Mode"));
            }
        }

        public static bool QueueNotification
        {
            get
            {
                return bool.Parse(RoleEnvironment.GetConfigurationSettingValue("FTP2Azure.QueueNotification"));
            }
        }

        public static int MaxIdleSeconds
        {
            get
            {
                return int.Parse(RoleEnvironment.GetConfigurationSettingValue("FTP2Azure.MaxIdleSeconds"));
            }
        }

        public static string FtpServerHost
        {
            get
            {
                return RoleEnvironment.GetConfigurationSettingValue("FTP2Azure.FtpServerHost");
            }
        }

    }
}