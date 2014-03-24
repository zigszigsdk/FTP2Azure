namespace AzureFtpServer.Ftp
{
    /// <summary>
    /// Gives a mechanism for classes to pass messages to the main form for display 
    /// in the messages list box
    /// </summary>
    public class FtpServerMessageHandler
    {
        #region Delegates

        public delegate void MessageEventHandler(int nId, string sMessage);

        #endregion

        protected FtpServerMessageHandler()
        {
        }

        public static event MessageEventHandler Message;

        public static void SendMessage(int nId, string sMessage)
        {
            if (Message != null)
            {
                Message(nId, sMessage);
            }
        }
    }
}