using AzureFtpServer.Azure;
using AzureFtpServer.Ftp.General;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;


namespace AzureFtpServer.Provider
{
    public sealed class AzureBlobStorageProvider
    {
        #region Member variables

        private CloudStorageAccount _account;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _container;

        #endregion

        #region Construction

        public AzureBlobStorageProvider(string containerName)
        {
            Initialise(containerName);
        }

        #endregion

        #region Properties

        public bool UseHttps { get; private set; }

        public string ContainerName { get; private set; }

        #endregion

        #region IStorageProvider Members

        /// <summary>
        /// Occurs when a storage provider operation has completed.
        /// </summary>
        //public event EventHandler<StorageProviderEventArgs> StorageProviderOperationCompleted;

        #endregion

        // Initialiser method
        private void Initialise(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentException("You must provide the base Container Name", nameof(containerName));

            ContainerName = containerName;

            _account = ResolveCloudStorageAccount();

            _blobClient = _account.CreateCloudBlobClient();
            _blobClient.DefaultRequestOptions = BlobRequestOptionsFactory.CreateDefault();
            _container = _blobClient.GetContainerReference(ContainerName);

            if (_container.Exists())
            {
                return;
            }

            Trace.WriteLine($"Create new container: {ContainerName}", "Information");
            _container.Create();

            // set new container's permissions
            // Create a permission policy to set the public access setting for the container. 
            var containerPermissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Off
            };

            // The public access setting explicitly specifies that the container is private,
            // so that it can't be accessed anonymously.

            //Set the permission policy on the container.
            Trace.WriteLine($"Set the permission policy: {ContainerName}", "Information");
            _container.SetPermissions(containerPermissions);
        }

        #region Storage operations

        public Stream GetWriteBlobStream(string path)
        {
            CloudBlockBlob blob = GetCloudBlob(path);

            Stream stream = blob?.OpenWrite();
            return stream;
        }

        public Stream GetReadBlobStream(string path)
        {
            ICloudBlob blob = GetCloudBlob(path);

            if (blob == null)
                return null;

            Stream stream = blob.OpenRead();
            stream.Position = 0;

            return stream;
        }

        private CloudBlockBlob GetCloudBlob(string path)
        {
            // convert to azure path
            string blobPath = path.ToAzurePath();

            // Create a reference for the filename
            // Note: won't check whether the blob exists
            CloudBlockBlob blob = _container.GetBlockBlobReference(blobPath);
            return blob;
        }

        /// <summary>
        /// Delete the specified file from the Azure container.
        /// </summary>
        /// <param name="path">the file to be deleted</param>
        public bool DeleteFile(string path)
        {
            if (!IsValidFile(path))
                return false;

            // convert to azure path
            string blobPath = path.ToAzurePath();

            ICloudBlob b = _container.GetBlockBlobReference(blobPath);
            if (b != null)
            {
                // Need AsyncCallback?
                b.DeleteIfExists();
            }
            else
            {
                Trace.WriteLine($"Get blob reference \"{path}\" failed", "Error");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Delete the specified directory from the Azure container.
        /// </summary>
        /// <param name="path">the directory path</param>
        public bool DeleteDirectory(string path)
        {
            if (!IsValidDirectory(path))
                return false;

            // cannot delete root directory
            if (path == "/")
                return false;

            IEnumerable<IListBlobItem> allFiles = _blobClient.ListBlobs(GetFullPath(path), true);
            foreach (var file in allFiles)
            {
                string uri = file.Uri.ToString();

                ICloudBlob b = _container.GetBlockBlobReference(uri);
                if (b != null)
                {
                    // Need AsyncCallback?
                    b.DeleteIfExists();
                }
                else
                {
                    Trace.WriteLine($"Get blob reference \"{uri}\" failed", "Error");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Retrieves the object from the storage provider
        /// </summary>
        /// <param name="path">the file/dir path in the FtpServer</param>
        /// <param name="isDirectory">whether path is a directory</param>
        /// <returns>AzureCloudFile</returns>
        /// <exception cref="FileNotFoundException">Throws a FileNotFoundException if the blob path is not found on the provider.</exception>
        public AzureCloudFile GetBlobInfo(string path, bool isDirectory)
        {
            // check parameter
            if (string.IsNullOrEmpty(path) || path[0] != '/')
                return null;

            // get the info of root directory
            if ((path == "/") && isDirectory)
                return new AzureCloudFile
                {
                    Uri = _container.Uri,
                    FtpPath = path,
                    IsDirectory = true,
                    Size = 1,
                    LastModified = DateTime.Now
                };

            // convert to azure path
            string blobPath = path.ToAzurePath();

            var azureCloudFile = new AzureCloudFile();

            try
            {
                if (isDirectory)
                {
                    CloudBlobDirectory bDir = _container.GetDirectoryReference(blobPath);
                    
                    // check whether directory exists
                    if (!bDir.ListBlobs().Any())
                        throw new StorageException(new RequestResult(), "directory is empty", new Exception());

                    azureCloudFile = new AzureCloudFile
                    {
                        Uri = bDir.Uri,
                        FtpPath = path,
                        IsDirectory = true,
                        // default value for size and modify time of directories
                        Size = 1,
                        LastModified = DateTime.Now
                    };
                }
                else
                {
                    ICloudBlob b = _container.GetBlockBlobReference(blobPath);
                    b.FetchAttributes();
                    azureCloudFile = new AzureCloudFile
                    {
                        Uri = b.Uri,
                        LastModified = b.Properties.LastModified.GetValueOrDefault(),
                        Size = b.Properties.Length,
                        FtpPath = path,
                        IsDirectory = false
                    };
                }

            }
            catch (StorageException)
            {
                Trace.WriteLine($"Get blob {path} failed", "Error");
                return null;
            }

            return azureCloudFile;
        }

        /// <summary>
        /// Gets the directory under the directory
        /// </summary>
        /// <param name="dirPath">directory path</param>
        /// <returns></returns>
        public IEnumerable<CloudBlobDirectory> GetDirectoryListing(string dirPath)
        {
            // Get the full path of directory
            string prefix = GetFullPath(dirPath);

            IEnumerable<CloudBlobDirectory> results = _blobClient.ListBlobs(prefix).OfType<CloudBlobDirectory>();

            return results;
        }

        /// <summary>
        /// List the files under the directory
        /// </summary>
        /// <param name="dirPath">directory path</param>
        /// <returns></returns>
        public IEnumerable<ICloudBlob> GetFileListing(string dirPath)
        {
            // Get the full path of directory
            string prefix = GetFullPath(dirPath);

            IEnumerable<ICloudBlob> results = _blobClient.ListBlobs(prefix).OfType<ICloudBlob>();

            return results;
        }

        /// <summary>
        /// Renames the specified object by copying the original to a new path and deleting the original.
        /// </summary>
        /// <param name="originalPath">The original path.</param>
        /// <param name="newPath">The new path.</param>
        /// <returns></returns>
        public StorageOperationResult Rename(string originalPath, string newPath)
        {
            CloudBlockBlob newBlob = _container.GetBlockBlobReference(newPath.ToAzurePath());
            CloudBlockBlob originalBlob = _container.GetBlockBlobReference(originalPath.ToAzurePath());

            // Check if the original path exists on the provider.
            if (!IsValidFile(originalPath))
            {
                throw new FileNotFoundException("The path supplied does not exist on the storage provider.",
                    originalPath);
            }

            newBlob.StartCopy(originalBlob);
            originalBlob.DeleteIfExists();
            return StorageOperationResult.Completed;
        }

        public bool CreateDirectory(string path)
        {
            path = path.ToAzurePath();

            string blobName = string.Concat(path, "required.req");

            try
            {
                ICloudBlob blob = _container.GetBlockBlobReference(blobName);

                string message = "#REQUIRED: At least one file is required to be present in this folder.";
                Stream msg = new MemoryStream(Encoding.UTF8.GetBytes(message));
                blob.UploadFromStream(msg);

                BlobProperties props = blob.Properties;
                props.ContentType = "text/text";
                blob.SetProperties();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified path is a valid blob folder.
        /// </summary>
        /// <param name="dirPath">a directory path, final char is '/'</param>
        /// <returns></returns>
        public bool IsValidDirectory(string dirPath)
        {
            if (dirPath == null)
                return false;

            // Important, when dirPath = "/", the behind HasDirectory(dirPath) will throw exceptions
            if (dirPath == "/")
                return true;

            // error check
            if (!dirPath.EndsWith(@"/"))
            {
                Trace.WriteLine($"Invalid parameter {dirPath} for function IsValidDirectory", "Error");
                return false;
            }

            // remove the first '/' char
            string blobDirPath = dirPath.ToAzurePath();

            // get reference
            CloudBlobDirectory blobDirectory = _container.GetDirectoryReference(blobDirPath);

            // non-exist blobDirectory won't contain blobs
            if (!blobDirectory.ListBlobs().Any())
                return false;

            return true;
        }

        /// <summary>
        /// Checks whether the specified path is a valid .
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public bool IsValidFile(string filePath)
        {
            if (filePath == null)
                return false;

            // error check
            if (filePath.EndsWith(@"/"))
            {
                Trace.WriteLine($"Invalid parameter {filePath} for function IsValidFile", "Error");
                return false;
            }

            // remove the first '/' char
            string fileBlobPath = filePath.ToAzurePath();

            try
            {
                ICloudBlob blob = _container.GetBlockBlobReference(fileBlobPath);
                blob.FetchAttributes();
            }
            catch (StorageException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// read bytes from the stream and append the content to an existed file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public bool AppendFileFromStream(string filePath, Stream stream)
        {
            filePath = filePath.ToAzurePath();

            ICloudBlob rawBlob = _container.GetBlockBlobReference(filePath);
            if (rawBlob.Properties.BlobType == BlobType.PageBlob)
            {
                return false; //for page blob, append is not supported
            }

            CloudBlockBlob blob = _container.GetBlockBlobReference(filePath);

            // store the block id of this blob
            var blockList = new List<string>();

            try
            {
                foreach (var block in blob.DownloadBlockList())
                {
                    blockList.Add(block.Name);
                }
            }
            catch (StorageException)
            {
                // do nothing, this may happen when blob doesn't exist
            }

            const int blockSize = 4 * 1024 * 1024; // 4M - block size
            byte[] buffer = new byte[blockSize];
            // append file
            try
            {
                int nRead = 0;
                while (nRead < blockSize)
                {
                    int actualRead = stream.Read(buffer, nRead, blockSize - nRead);
                    if (actualRead <= 0) // stream end
                    {
                        //put last block & break
                        string strBlockId = GetBlockID(blockList);
                        blob.PutBlock(strBlockId, new MemoryStream(buffer, 0, nRead), null);
                        blockList.Add(strBlockId);
                        break;
                    }
                    else if (actualRead == (blockSize - nRead)) // buffer full
                    {
                        //put this block
                        string strBlockId = GetBlockID(blockList);
                        blob.PutBlock(strBlockId, new MemoryStream(buffer), null);
                        blockList.Add(strBlockId);
                        nRead = 0;
                        continue;
                    }
                    nRead += actualRead;
                }
            }
            catch (StorageException)
            {
                // blob.PutBlock error
                return false;
            }

            // put block list
            blob.PutBlockList(blockList);

            return true;
        }

        /// <summary>
        /// After successfully upload a new file, user Azure queue to record it
        /// </summary>
        /// <param name="filePath">the path of the new file</param>
        public void UploadNotification(string filePath)
        {
            if (!StorageProviderConfiguration.QueueNotification)
                return;

            // Create the queue client
            CloudQueueClient queueClient = _account.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            CloudQueue queue = queueClient.GetQueueReference("ftp2azure-queue");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            // Get the new blob's URI
            // remove the first '/' char
            string fileBlobPath = filePath.ToAzurePath();
            ICloudBlob blob = _container.GetBlockBlobReference(fileBlobPath);

            // Create a message and add it into the queue
            CloudQueueMessage message = new CloudQueueMessage($"User uploaded blob: {blob.Uri}");
            queue.AddMessage(message);
        }

        /// <summary>
        /// Set blob ContentMD5
        /// </summary>
        /// <param name="filePath">blob path</param>
        /// <param name="md5Value"></param>
        public void SetBlobMd5(string filePath, string md5Value)
        {
            // get the blob
            // remove the first '/' char
            string fileBlobPath = filePath.ToAzurePath();
            ICloudBlob blob = _container.GetBlockBlobReference(fileBlobPath);

            blob.FetchAttributes();

            blob.Properties.ContentMD5 = md5Value;

            blob.SetProperties();
        }

        /// <summary>
        /// Set blob MimeType
        /// </summary>
        /// <param name="filePath">blob path</param>
        /// <param name="mimeType"></param>
        public void SetFileMimeType(string filePath, string mimeType)
        {
            // get the blob
            // remove the first '/' char
            string fileBlobPath = filePath.ToAzurePath();
            ICloudBlob blob = _container.GetBlockBlobReference(fileBlobPath);

            blob.FetchAttributes();
            blob.Properties.ContentType = mimeType;

            blob.SetProperties();
        }

        #endregion

        #region "Helper methods"

        /// <summary>
        /// Get the full path (as in URI) of a blob folder or file
        /// </summary>
        /// <param name="path">a folder path or a file path, absolute path</param>
        /// <returns></returns>
        private string GetFullPath(string path)
        {
            return ContainerName + path;
        }

        private string GetBlockID(List<string> currentIds)
        {
            string blockID = null;

            while (true)
            {
                string tempStr = Convert.ToBase64String(Encoding.ASCII.GetBytes(DateTime.Now.ToBinary().ToString()));
                int idLength = (!currentIds.Any()) ? 64 : currentIds[0].Length;
                tempStr = TextHelpers.RightAlignString(tempStr, idLength, 'A');
                bool sameId = false;
                foreach (var id in currentIds)
                {
                    if (id == tempStr)
                    {
                        sameId = true;
                        break;
                    }
                }
                if (!sameId)
                {
                    blockID = tempStr;
                    break;
                }
            }

            return blockID;
        }

        private void _GetBlobMd5(string filePath)
        {
            string fileBlobPath = filePath.ToAzurePath();
            ICloudBlob blob = _container.GetBlockBlobReference(fileBlobPath);

            blob.FetchAttributes();

            Trace.WriteLine("GetMd5#" + blob.Properties.ContentMD5);
        }

        private static CloudStorageAccount ResolveCloudStorageAccount()
        {
            return StorageProviderConfiguration.Mode == Modes.Debug
                ? CloudStorageAccount.DevelopmentStorageAccount
                : CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("FTP2Azure.StorageAccount"));
        }

        #endregion

        #region "Callbacks"


        #endregion
    }
}
