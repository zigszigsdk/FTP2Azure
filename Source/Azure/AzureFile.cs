using System;
using System.IO;
using AzureFtpServer.Ftp.FileSystem;


namespace AzureFtpServer.Azure
{
    public sealed class AzureFile : IFile
    {

        public Stream stream { get; set; }

        public int Read(byte[] abData, int nDataSize)
        {
            if (stream == null)
            {
                return 0;
            }

            try
            {
                return stream.Read(abData, 0, nDataSize);
            }
            catch (IOException)
            {
                return 0;
            }
            // other exceptions
            catch (Exception)
            {
                // need logging, fix me
                return 0;
            }
        }

        public int Write(byte[] abData, int nDataSize)
        {
            if (stream == null)
            {
                throw new Exception("BlobStream is null!");
            }

            try
            {
                stream.Write(abData, 0, nDataSize);
                return nDataSize;
            }
            catch (IOException)
            {
                return 0;
            }
        }

        public void Close()
        {
            if (stream != null)
            {
                try
                {
                    stream.Close();
                }
                catch (IOException)
                {
                }

                stream = null;
            }
        }

    }
}