using System;
using AzureFtpServer.Ftp.General;

namespace AzureFtpServer.Azure
{
    public static class ExtensionMethods
    {
        // Convert a string instance of a URI to a relative FTP path
        // Return the file name or the folder name
        public static string ToFtpPath(string s)
        {
            bool isDir = s.EndsWith(@"/");

            if (isDir)
            {
                // Note: append the directory end tag "/"
                return FileNameHelpers.GetDirectoryName(s) + "/";
            }
            // the path is a file
            else
            {
                return FileNameHelpers.GetFileName(s);
            }
        }

        // Convert an array of string URI paths to a string array of FTP paths
        public static string[] ToFtpPath(this string[] paths, string dirPath)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = dirPath + ToFtpPath(paths[i]);
            }
            return paths;
        }

        // Convert a ftp path to a azure path
        public static string ToAzurePath(this string path)
        {
            if ((path == null) || (path.Length <= 1) || (path[0] != '/'))
                throw new Exception("Invalid argument for function:ToAzurePath");
            return path.Remove(0, 1);
        }
    }
}