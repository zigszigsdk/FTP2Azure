using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using AzureFtpServer.Ftp.FileSystem;
using AzureFtpServer.Provider;
using Microsoft.WindowsAzure.Storage.Blob;
using AzureFtpServer.Ftp.General;

namespace AzureFtpServer.Azure
{
    public class AzureFileSystem : IFileSystem
    {
        private readonly AzureBlobStorageProvider _provider;
        private string _containerName;

        // Constructor
        public AzureFileSystem(string containerName)
        {
            // Set container name (if none specified, specify the development container default)
            _containerName = !string.IsNullOrEmpty(containerName) ? containerName : "DevelopmentContainer";
            _provider = new AzureBlobStorageProvider(_containerName);
        }

        #region Implementation of IFileSystem

        public IFile OpenFile(string sPath, bool fWrite)
        {
            var f = new AzureFile();
            if (fWrite == true)
            {
                f.stream = _provider.GetWriteBlobStream(sPath);
            }
            else
            {
                f.stream = _provider.GetReadBlobStream(sPath);
            }

            if (f.stream == null)
                return null;

            return f;
        }

        public IFileInfo GetFileInfo(string sPath)
        {
            AzureCloudFile file = _provider.GetBlobInfo(sPath, false);
            
            return new AzureFileInfo(file);
        }

        public IFileInfo GetDirectoryInfo(string sDirPath)
        {
            AzureCloudFile dir = _provider.GetBlobInfo(sDirPath, true);
            
            return new AzureFileInfo(dir);
        }

        /// <summary>
        /// Get the filename list in the directory 
        /// </summary>
        /// <param name="sDirPath">directory path</param>
        /// <returns>an arry of filenames</returns>
        public string[] GetFiles(string sDirPath)
        {
            IEnumerable<ICloudBlob> files = _provider.GetFileListing(sDirPath);
            string[] result = files.Select(r => r.Uri.AbsolutePath.ToString()).ToArray().ToFtpPath(sDirPath);
            result = FilterOutRequiredReqFile(result);
            return result;
        }

        string[] FilterOutRequiredReqFile(string[] filesWithPath) 
        {
            List<string> result = new List<string>();
            foreach (string filename in filesWithPath)
                if (FileNameHelpers.GetFileName(filename) != "required.req")
                    result.Add(filename);

            return result.ToArray();
        }

        public string[] GetAllFilesInSubs(string sDirPath) 
        {
            string[] result = { };

            string[] sDirPaths = GetAllSubDirectoriesIn(new string[] { sDirPath });

            foreach (string path in sDirPaths) 
                result = StringArrayConcat(result, GetFiles(path));
            
            return result;
        }

        /// <summary>
        /// Get the directory name list in the directory 
        /// </summary>
        /// <param name="sDirPath">directory path</param>
        /// <returns>an arry of directorynames</returns>
        public string[] GetDirectories(string sDirPath)
        {
            IEnumerable<CloudBlobDirectory> directories = _provider.GetDirectoryListing(sDirPath);
            string[] result =  directories.Select(r => r.Uri.AbsolutePath.ToString()).ToArray().ToFtpPath(sDirPath);
            return result;
        }
        public string[] GetAllSubDirectoriesIn(string[] sDirPaths)
        {
            foreach (string path in sDirPaths)
                sDirPaths = StringArrayConcat(sDirPaths, GetAllSubDirectoriesIn(GetDirectories(path)));

            return sDirPaths;
        }

        string[] StringArrayConcat(string[] x, string[] y) 
        {
            string[] res = new string[x.Length + y.Length];
            x.CopyTo(res, 0);
            y.CopyTo(res, x.Length);
            return res;
        }


        /// <summary>
        /// check if the directory exists
        /// </summary>
        /// <param name="sPath">the directory name, final char is '/'</param>
        /// <returns></returns>
        public bool DirectoryExists(string sDirPath)
        {
            return _provider.IsValidDirectory(sDirPath);
        }

        /// <summary>
        /// check if the file exists
        /// </summary>
        /// <param name="sPath">the file name</param>
        /// <returns></returns>
        public bool FileExists(string sPath)
        {
            return _provider.IsValidFile(sPath);
        }

        public bool CreateDirectory(string sPath)
        {
            return _provider.CreateDirectory(sPath);
        }

        public bool Move(string sOldPath, string sNewPath)
        {
            return _provider.Rename(sOldPath, sNewPath) == StorageOperationResult.Completed;
        }

        public bool DeleteFile(string sPath)
        {
            return _provider.DeleteFile(sPath);
        }

        public bool DeleteDirectory(string sPath)
        {
            return _provider.DeleteDirectory(sPath);
        }

        public bool AppendFile(string sPath, Stream stream)
        {
            return _provider.AppendFileFromStream(sPath, stream);
        }

        public void Log4Upload(string sPath)
        {
            _provider.UploadNotification(sPath);
        }

        public void SetFileMd5(string sPath, string md5Value)
        {
            _provider.SetBlobMd5(sPath, md5Value);
        }

        #endregion
    }
}
