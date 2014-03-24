using System.Diagnostics;
using AzureFtpServer.Ftp;
using AzureFtpServer.General;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// Base class for all ftp command handlers.
    /// </summary>
    internal class FtpCommandHandler
    {
        #region Member Variables

        private readonly string m_sCommand;
        private readonly FtpConnectionObject m_theConnectionObject;

        #endregion

        #region Construction

        protected FtpCommandHandler(string sCommand, FtpConnectionObject connectionObject)
        {
            m_sCommand = sCommand;
            m_theConnectionObject = connectionObject;
        }

        #endregion

        #region Properties

        public string Command
        {
            get { return m_sCommand; }
        }

        public FtpConnectionObject ConnectionObject
        {
            get { return m_theConnectionObject; }
        }

        #endregion

        #region Methods

        public void Process(string sMessage)
        {
            SendMessage(OnProcess(sMessage));
        }

        protected virtual string OnProcess(string sMessage)
        {
            Debug.Assert(false, "FtpCommandHandler::OnProcess base called");
            return "";
        }

        protected string GetMessage(int nReturnCode, string sMessage)
        {
            return string.Format("{0} {1}\r\n", nReturnCode, sMessage);
        }

        /// <summary>
        /// Get the full path of sPath, will use m_theConnectionObject.CurrentDirectory
        /// </summary>
        /// <param name="sPath">path name, foler or file</param>
        /// <returns></returns>
        protected string GetPath(string sPath)
        {
            string current = m_theConnectionObject.CurrentDirectory;

            if (sPath.Length == 0)
            {
                return current;
            }

            // sPath is an absolute path
            if (sPath[0] == '/')
                return sPath;
            // sPath is a relative path
            // Notice: current's last char is '/'
            else
            {
                if (sPath.StartsWith(@"./")) // support "./", not support "../"
                    return GetPath(sPath.Remove(0, 2));
                return current + sPath;
            }
        }

        /// <summary>
        /// Get the parent directory of CurrentDirectory
        /// </summary>
        /// <returns>null if current is root dir</returns>
        protected string GetParentDir()
        {
            string current = m_theConnectionObject.CurrentDirectory;
            
            // current is root dir
            if (current == @"/")
                return null;

            // remove the last child directory
            int lastDirectoryDelim = current.Remove(current.Length - 1).LastIndexOf('/');
            if (lastDirectoryDelim > -1) // must be true
                current = current.Substring(0, lastDirectoryDelim + 1);
            else
            {
                FtpServerMessageHandler.SendMessage(ConnectionObject.Id, string.Format("Invalid current dir:{0}", current));
                return null;
            }

            return current;
        }

        private void SendMessage(string sMessage)
        {
            if (sMessage.Length == 0)
            {
                return;
            }

            int nEndIndex = sMessage.IndexOf('\r');

            if (nEndIndex < 0)
            {
                FtpServerMessageHandler.SendMessage(m_theConnectionObject.Id, sMessage);
            }
            else
            {
                FtpServerMessageHandler.SendMessage(m_theConnectionObject.Id, sMessage.Substring(0, nEndIndex));
            }

            SocketHelpers.Send(ConnectionObject.Socket, sMessage, ConnectionObject.Encoding);
        }

        #endregion
    }
}