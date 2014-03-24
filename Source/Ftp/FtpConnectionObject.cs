using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.FtpCommands;
using AzureFtpServer.Ftp;
using AzureFtpServer.General;

namespace AzureFtpServer.Ftp
{
    /// Processes incoming messages and passes the data on to the relevant handler class.
    internal class FtpConnectionObject : FtpConnectionData
    {

        private readonly IFileSystemClassFactory m_fileSystemClassFactory;
        private readonly Hashtable m_theCommandHashTable;
        private bool isLogged;
        private static bool m_useDataSocket;

        public event AzureFtpServer.Ftp.FtpServer.UserAccessEvent UserLoginEvent;
        public event AzureFtpServer.Ftp.FtpServer.UserAccessEvent UserLogoutEvent;

        public bool DataSocketOpen
        { 
            get { return m_useDataSocket; }
        }





        public FtpConnectionObject(IFileSystemClassFactory fileSystemClassFactory, int nId, TcpClient socket)
            : base(nId, socket)
        {
            m_theCommandHashTable = new Hashtable();
            m_fileSystemClassFactory = fileSystemClassFactory;
            isLogged = false;
            m_useDataSocket = false;
            LoadCommands();
        }

        ~FtpConnectionObject() 
        {
            LogOut();
        }


        public bool Login(string sPassword)
        {
            if ((User == null) || (sPassword == null))
            {
                return false;
            }

            IFileSystem fileSystem = m_fileSystemClassFactory.Create(User, sPassword);

            if (fileSystem == null) //user and pass combo doesn't exist
            {
                return false;
            }

            if (UserLoginEvent != null)
                UserLoginEvent(User);

            SetFileSystemObject(fileSystem);
            isLogged = true;
            return true;
        }

        private void LoadCommands()
        {
            //RFC959: Base Commands
            AddCommand(new AbortCommandHandler(this));// stop data connection
            AddCommand(new AccountCommandHandler(this));
            AddCommand(new AlloCommandHandler(this));
            AddCommand(new AppendCommandHandler(this));
            AddCommand(new CdupCommandHandler(this));
            AddCommand(new CwdCommandHandler(this));
            AddCommand(new DeleCommandHandler(this));
            AddCommand(new HelpCommandHandler(this));
            AddCommand(new ListCommandHandler(this));
            AddCommand(new MakeDirectoryCommandHandler(this));
            AddCommand(new ModeCommandHandler(this));
            AddCommand(new NlstCommandHandler(this));
            AddCommand(new NoopCommandHandler(this));
            AddCommand(new PasswordCommandHandler(this));
            AddCommand(new PasvCommandHandler(this));
            AddCommand(new PortCommandHandler(this));
            AddCommand(new PwdCommandHandler(this));
            AddCommand(new QuitCommandHandler(this));
            AddCommand(new ReinitializeCommandHandler(this));
            AddCommand(new RestartCommandHandler(this));// not imp
            AddCommand(new RetrCommandHandler(this));
            AddCommand(new RemoveDirectoryCommandHandler(this));
            AddCommand(new RenameStartCommandHandler(this));
            AddCommand(new RenameCompleteCommandHandler(this));
            AddCommand(new SiteCommandHandler(this));
            AddCommand(new SmntCommandHandler(this));
            AddCommand(new StatCommandHandler(this));
            AddCommand(new StoreCommandHandler(this));
            AddCommand(new StructureCommandHandler(this));
            AddCommand(new StouCommandHandler(this));
            AddCommand(new SystemCommandHandler(this));
            AddCommand(new TypeCommandHandler(this));
            AddCommand(new UserCommandHandler(this));
            

            //Obsolete commands
            AddCommand(new XCdupCommandHandler(this));
            AddCommand(new XCwdCommandHandler(this));
            AddCommand(new XMkdCommandHandler(this));
            AddCommand(new XPwdCommandHandler(this));
            AddCommand(new XRmdCommandHandler(this));

            //Other commands
            AddCommand(new FeatCommandHandler(this));
            AddCommand(new MdtmCommandHandler(this));
            AddCommand(new MlsdCommandHandler(this));
            AddCommand(new MlstCommandHandler(this));
            AddCommand(new SizeCommandHandler(this));
     
        }

        private void AddCommand(FtpCommandHandler handler)
        {
            m_theCommandHashTable.Add(handler.Command, handler);
        }

        public void Process(Byte[] abData)
        {
            string sMessage = this.Encoding.GetString(abData);
            sMessage = sMessage.Substring(0, sMessage.IndexOf('\r'));

            FtpServerMessageHandler.SendMessage(Id, sMessage);

            string sCommand;
            string sValue;

            int nSpaceIndex = sMessage.IndexOf(' ');

            if (nSpaceIndex < 0)
            {
                sCommand = sMessage.ToUpper();
                sValue = "";
            }
            else
            {
                sCommand = sMessage.Substring(0, nSpaceIndex).ToUpper();
                sValue = sMessage.Substring(sCommand.Length + 1);
            }

            // check whether the client has logged in
            if (!isLogged)
            {
                if (!((sCommand == "USER") || (sCommand == "PASS") || (sCommand == "HELP") || (sCommand == "FEAT") || (sCommand == "QUIT")))
                {
                    SocketHelpers.Send(Socket, "530 Not logged in\r\n", this.Encoding);
                    return;
                }
            }

            // check if data connection will be used
            if ((sCommand == "APPE") || (sCommand == "MLSD") || (sCommand == "LIST") || (sCommand == "RETR") || (sCommand == "STOR"))
            {
                m_useDataSocket = true;
            }

            var handler = m_theCommandHashTable[sCommand] as FtpCommandHandler;

            if (handler == null)
            {
                FtpServerMessageHandler.SendMessage(Id, string.Format("\"{0}\" : Unknown command", sCommand));
                SocketHelpers.Send(Socket, "550 Unknown command\r\n", this.Encoding);
            }
            else
            {
                handler.Process(sValue);
            }

            // reset
            m_useDataSocket = false;
        }

        public void LogOut()
        {
            isLogged = false;

            if (UserLogoutEvent != null)
                UserLogoutEvent(User);

            //TODO: stop current cmd & close data connection

            // reinitialize all parameters
            SetFileSystemObject(null);
            CurrentDirectory = "/";
            User = null;
            FileToRename = null;
            DataConnectionType = DataConnectionType.Invalid;
            DataType = DataType.Image;

            // currently won't change
            TransmissionMode = TransmissionMode.Stream;
            DataStructure = DataStructure.File;
            FormatControl = FormatControl.NonPrint;
        }

    }
}