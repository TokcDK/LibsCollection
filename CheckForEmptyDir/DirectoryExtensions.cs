using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CheckForEmptyDir
{
    public static class DirectoryExtensions
    {
        static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WIN32_FIND_DATA
        {
            public FileAttributes dwFileAttributes;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public int dwReserved0;
            public int dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll")]
        private static extern bool FindClose(IntPtr hFindFile);

        /// <summary>
        /// return true if path nul,aempty or not contains any dirs/files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mask"></param>
        /// <param name="exclusions"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyDirectory(this DirectoryInfo path, string mask = "*", string[] exclusions = null, bool recursive = false, bool searchForFiles = true, bool searchForDirs = true, bool isSubDir = false)
        {
            return path.FullName.IsNullOrEmptyDirectory(mask: mask, exclusions: exclusions, recursive: recursive, searchForFiles: searchForFiles, searchForDirs: searchForDirs, isSubDir: isSubDir);
        }

        //info: https://stackoverflow.com/a/757925
        //быстро проверить, пуста ли папка
        /// <summary>
        /// return true if path nul,aempty or not contains any dirs/files
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mask"></param>
        /// <param name="exclusions"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyDirectory(this string path, string mask = "*", string[] exclusions = null, bool recursive = false, bool searchForFiles = true, bool searchForDirs = false, bool isSubDir = false)
        {
            if (!isSubDir) // make it only 1st time
            {
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                {
                    return true; //return true if path is empty
                                 //throw new ArgumentNullException(path);
                }

                // commented because perfomance issues and even with commented invalid link will be parsed as InvalidHandleValue and wil return true as for empty dir
                //if (path.IsSymLink() && !path.IsValidSymlink()) // check if target is symlink and it is invalid. perfomance issue here
                //{
                //    return true;
                //}

                path = path.TrimEnd(Path.DirectorySeparatorChar);
            }

            var findHandle = FindFirstFile(path + Path.DirectorySeparatorChar + mask, out WIN32_FIND_DATA findData);

            if (findHandle == INVALID_HANDLE_VALUE) return true;
            try
            {
                bool empty = true;
                int skipCnt = 2; // for skip root and parent dirs without need to compare strings
                do
                {
                    bool isDir;
                    if ((isSubDir && (skipCnt--) > 0) // replace of 2 checks below for . and ..
                        || (!isSubDir && findData.cFileName == "." /*root dir*/ || findData.cFileName == ".." /*parent dir*/)
                        || findData.cFileName.ContainsAnyFrom(exclusions)
                        || (((isDir = IsDir(findData.dwFileAttributes)) && searchForFiles) || (searchForDirs && !isDir)) // skip dir when need to find files or skip file when need to find dirs
                        || (recursive && !searchForDirs && isDir && IsNullOrEmptyDirectory(path + Path.DirectorySeparatorChar + findData.cFileName, mask, exclusions, recursive, isSubDir: isSubDir))) // recursive and subfolder is empty
                                                                                                                                                                                                       //&& mask.Length != 1 && !findData.cFileName.EndsWith(mask.Remove(0, 1), StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                    empty = false;
                } while (empty && FindNextFile(findHandle, out findData));

                return empty;
            }
            finally
            {
                FindClose(findHandle);
            }
        }

        /// <summary>
        /// true if any word from exclusions is containing in inputString
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="exclusions"></param>
        /// <returns></returns>
        static bool ContainsAnyFrom(this string inputString, string[] exclusions)
        {
            if (inputString.Length > 0 && exclusions != null)
            {
                int exclusionsLength = exclusions.Length;
                for (int i = 0; i < exclusionsLength; i++)
                {
                    if (inputString.Contains(exclusions[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// true if directory attributes
        /// </summary>
        /// <param name="dwFileAttributes"></param>
        /// <returns></returns>
        static bool IsDir(FileAttributes dwFileAttributes)
        {
            return (dwFileAttributes & FileAttributes.Directory) != 0;
        }
    }
}
