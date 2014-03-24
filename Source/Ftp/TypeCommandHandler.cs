using AzureFtpServer.Ftp;

namespace AzureFtpServer.FtpCommands
{
    /// <summary>
    /// TYPE command handler
    /// set the data type of this connection
    /// </summary>
    internal class TypeCommandHandler : FtpCommandHandler
    {
        public TypeCommandHandler(FtpConnectionObject connectionObject)
            : base("TYPE", connectionObject)
        {
        }

        protected override string OnProcess(string sMessage)
        {
            string[] args = sMessage.Split(' ');
            
            if (args.Length > 2 || args.Length < 1)
            {
                return GetMessage(501, string.Format("Invalid TYPE parameter: {0}", sMessage));
            }

            switch (args[0].ToUpper())
            { 
                case "A":
                    ConnectionObject.DataType = DataType.Ascii;
                    if (args.Length == 1)
                    {
                        return GetMessage(200, string.Format("{0} command succeeded, data type is Ascii", Command));
                    }
                    else 
                    {
                        switch (args[1].ToUpper())
                        { 
                            case "N":
                                ConnectionObject.FormatControl = FormatControl.NonPrint;
                                return GetMessage(200, string.Format("{0} command succeeded, data type is Ascii, format is NonPrint", Command));
                            case "T":
                            case "C":
                                ConnectionObject.FormatControl = FormatControl.NonPrint;
                                return GetMessage(504, string.Format("Format {0} is not supported, use NonPrint format", args[1]));
                            default:
                                return GetMessage(550, string.Format("Error - unknown format \"{0}\"", args[1]));
                        }
                    }
                case "I":
                    ConnectionObject.DataType = DataType.Image;
                    return GetMessage(200, string.Format("{0} command succeeded, data type is Image (Binary)", Command));
                case "E":
                case "L":
                    ConnectionObject.DataType = DataType.Image;
                    return GetMessage(504, string.Format("Data type {0} is not supported, use Image (Binary) type", args[0]));
                default:
                    return GetMessage(550, string.Format("Error - unknown data type \"{0}\"", args[1]));
            }
        }
    }
}