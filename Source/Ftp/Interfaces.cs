using System;
using System.IO;

namespace AzureFtpServer.Ftp.FileSystem
{
    public interface IFile
    {
        Stream stream { get; set; }
        int Read(byte[] abData, int nDataSize);
        int Write(byte[] abData, int nDataSize);
        void Close();
    }

    public interface IFileInfo
    {
        DateTimeOffset GetModifiedTime();
        long GetSize();
        string GetAttributeString();
        bool IsDirectory();
        string Path();
        bool FileObjectExists();
    }

    public interface IFileSystem
    {
        IFile OpenFile(string sPath, bool fWrite);
        IFileInfo GetFileInfo(string sPath);
        IFileInfo GetDirectoryInfo(string sPath);

        string[] GetFiles(string sDirPath);
        string[] GetAllFilesInSubs(string sDirPaths);
        string[] GetDirectories(string sDirPath);

        string[] GetAllSubDirectoriesIn(string[] sDirPath);

        bool DirectoryExists(string sDirPath);
        bool FileExists(string sPath);

        bool CreateDirectory(string sPath);
        bool Move(string sOldPath, string sNewPath); // file, not directory
        bool DeleteFile(string sPath);

        bool DeleteDirectory(string sPath);
        bool AppendFile(string sPath, Stream stream);

        void Log4Upload(string sPath); // upload notification
        void SetFileMd5(string sPath, string md5Value); // record md5 for upload files

        void SetFileMimeType(string sPath, string mimeType); // record correct mime type for upload files
    }

    public interface IFileSystemClassFactory
    {
        IFileSystem Create(string sUser, string sPassword);
    }
}
