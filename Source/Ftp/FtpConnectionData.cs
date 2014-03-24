using System.Net.Sockets;
using System.Text;
using AzureFtpServer.Ftp.FileSystem;

namespace AzureFtpServer.Ftp
{



    /// Data representations are handled in FTP by a user specifying a representation type
    enum DataType
    {
        Ascii,
        Ebcdic,
        Image,
        Local
    }


    /// The types ASCII and EBCDIC have different kinds of vertical formats
    enum FormatControl
    {
        NonPrint,
        Telnet,
        Carriage
    }


    /// structure of a ftp file
    enum DataStructure
    {
        File,
        Record,
        Page
    }

    /// when transfer data, choose the appropriate transmission mode
    enum TransmissionMode
    {
        Stream,
        Block,
        Compressed
    }

    /// The data transfer process (DTP) establishes and manages the data connection
    enum DataConnectionType
    {
        Active,
        Passive,
        Invalid
    }


    /// Contains all the data relating to a particular FTP connection
    internal class FtpConnectionData
    {
        private readonly int m_nId;
        private readonly TcpClient m_theSocket;
        private IFileSystem m_fileSystem;
        private int m_nPortCommandSocketPort = 20;
        private string m_sPortCommandSocketAddress = "";
        private TcpClient m_passiveSocket;
        private string m_sCurrentDirectory = "/"; // default is the root dir

        private DataType m_dataType = DataType.Image;
        private FormatControl m_formatControl = FormatControl.NonPrint;
        private DataStructure m_dataStructure = DataStructure.File;
        private TransmissionMode m_transMode = TransmissionMode.Stream;
        private DataConnectionType m_dataConnectionType = DataConnectionType.Invalid;


        public FtpConnectionData(int nId, TcpClient socket)
        {
            m_nId = nId;
            m_theSocket = socket;
        }


        /// Main connection socket
        public TcpClient Socket
        {
            get { return m_theSocket; }
        }

        public string User { get; set; }

        /// This connection's current directory
        /// E.g. "/", "/subdir/", "/subdir/subsubdir/"
        /// The last char in CurrentDirectory is '/'
        public string CurrentDirectory
        {
            get { return m_sCurrentDirectory; }

            set { m_sCurrentDirectory = value; }
        }

        /// <summary>
        /// This connection's Id
        /// </summary>
        public int Id
        {
            get { return m_nId; }
        }

        /// <summary>
        /// Socket address from PORT command.
        /// See FtpReplySocket class.
        /// </summary>
        public string PortCommandSocketAddress
        {
            get { return m_sPortCommandSocketAddress; }

            set { m_sPortCommandSocketAddress = value; }
        }

        /// <summary>
        /// Port from PORT command.
        /// See FtpReplySocket class.
        /// </summary>
        public int PortCommandSocketPort
        {
            get { return m_nPortCommandSocketPort; }

            set { m_nPortCommandSocketPort = value; }
        }

        /// <summary>
        /// The client's data connnection socket in passive mode
        /// </summary>
        public TcpClient PassiveSocket
        { 
            get { return m_passiveSocket; }
            
            set { m_passiveSocket = value; } 
        }

        /// <summary>
        /// Encoding to parse client's cmd and encode server's control channel responses
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// DataType from TYPE command
        /// Currently, only support Ascii and Image(binary)
        /// </summary>
        public DataType DataType
        {
            get { return m_dataType; }

            set { m_dataType = value; }
        }

        /// <summary>
        /// FormatControl from TYPE command (in Ascii or Ebcdic type)
        /// Currently, only support NonPrint
        /// </summary>
        public FormatControl FormatControl
        {
            get { return m_formatControl; }

            set { m_formatControl = value; }
        }

        /// <summary>
        /// DataStructure from STRU command
        /// Currently, only support File
        /// </summary>
        public DataStructure DataStructure
        {
            get { return m_dataStructure; }

            set { m_dataStructure = value; }
        }

        /// <summary>
        /// TransmissionMode from MODE command
        /// Currently, only support Stream
        /// </summary>
        public TransmissionMode TransmissionMode
        {
            get { return m_transMode; }

            set { m_transMode = value; }
        }

        /// <summary>
        /// DataConnectionType from PORT or PASV command
        /// </summary>
        public DataConnectionType DataConnectionType
        {
            get { return m_dataConnectionType; }

            set { m_dataConnectionType = value; }
        }

        /// <summary>
        /// Rename takes place with 2 commands - we store the old name here
        /// </summary>
        public string FileToRename { get; set; }

        /// <summary>
        /// This is true if the file to rename is a directory
        /// </summary>
        //public bool RenameDirectory { get; set; }

        public IFileSystem FileSystemObject
        {
            get { return m_fileSystem; }
        }

        protected void SetFileSystemObject(IFileSystem fileSystem)
        {
            m_fileSystem = fileSystem;
        }

    }
}