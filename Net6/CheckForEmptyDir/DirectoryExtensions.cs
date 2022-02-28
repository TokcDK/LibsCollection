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
        private static extern IntPtr FindFirstFileW(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool FindNextFileW(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll")]
        private static extern bool FindClose(IntPtr hFindFile);

        /// <summary>
        /// return true if path nul,empty or not contains any dirs/files.
        /// be aware that all file/dir names containing keywords from <paramref name="keywordsOfIgnoringNames"/> will be ignored as if they missing in the folder.
        /// </summary>
        /// <param name="path">input dir which must be checked</param>
        /// <param name="mask">mask of file/dir names in <paramref name="path"/> dir</param>
        /// <param name="keywordsOfIgnoringNames">file\dir name will be ignored if contains and keyword from this list</param>
        /// <param name="recursive">means need to search in subdirs. only need for when  <paramref name="searchForFiles" is true/></param>
        /// <param name="searchForFiles">means any files wil be searched by the <paramref name="mask"/> in <paramref name="path"/> dir or in any subdirs when <paramref name="recursive"/> is true</param>
        /// <param name="searchForDirs">means any subdirs wil be searched by the <paramref name="mask"/></param>
        /// <param name="isSubDir">means that is subdir, usually called from this method</param>
        /// <returns></returns>
        public static bool IsNullOrEmptyDirectory(this DirectoryInfo path, string mask = "*", string[] keywordsOfIgnoringNames = null, bool recursive = true, bool searchForFiles = true, bool searchForDirs = true, bool isSubDir = false)
        {
            return path.FullName.IsNullOrEmptyDirectory(mask: mask, keywordsOfIgnoringNames: keywordsOfIgnoringNames, recursive: recursive, searchForFiles: searchForFiles, searchForDirs: searchForDirs, isSubDir: isSubDir);
        }

        //info: https://stackoverflow.com/a/757925
        //быстро проверить, пуста ли папка
        /// <summary>
        /// return true if path nul,empty or not contains any dirs/files.
        /// be aware that all file/dir names containing keywords from <paramref name="keywordsOfIgnoringNames"/> will be ignored as if they missing in the folder.
        /// </summary>
        /// <param name="path">input dir which must be checked</param>
        /// <param name="mask">mask of file/dir names in <paramref name="path"/> dir</param>
        /// <param name="keywordsOfIgnoringNames">file\dir name will be ignored if contains and keyword from this list</param>
        /// <param name="recursive">means need to search in subdirs. only need for when  <paramref name="searchForFiles" is true/></param>
        /// <param name="searchForFiles">means any files wil be searched by the <paramref name="mask"/> in <paramref name="path"/> dir or in any subdirs when <paramref name="recursive"/> is true</param>
        /// <param name="searchForDirs">means any subdirs wil be searched by the <paramref name="mask"/></param>
        /// <param name="isSubDir">means that is subdir, usually called from this method</param>
        /// <param name="preciseMask">means <paramref name="mask"/> is equal full file or dir name like "filename.ext" not contains any "*" chars</param>
        /// <returns></returns>
        public static bool IsNullOrEmptyDirectory(this string path, string mask = "*", string[] keywordsOfIgnoringNames = null, bool recursive = true, bool searchForFiles = true, bool searchForDirs = false, bool isSubDir = false, bool preciseMask = false)
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
                preciseMask = preciseMask || mask.IndexOf('*') == -1 && mask.IndexOf('?') == -1; // when searching for precise file and mask equals something like "filename.txt"
                path = path.TrimEnd(Path.DirectorySeparatorChar);
            }

            var findHandle = FindFirstFileW(path + Path.DirectorySeparatorChar + mask, out WIN32_FIND_DATA findData); // will lock dir
            bool defaultMask = (mask == "*");
            try
            {
                if (!defaultMask && findHandle != INVALID_HANDLE_VALUE) return false; // return only for not "*" mask because there also .. and . folders

                if (recursive && searchForFiles && findHandle == INVALID_HANDLE_VALUE) // only when recursive and search for files
                {
                    // mask must be "*" in to not skip subfolders if search for file
                    findHandle = FindFirstFileW(path + Path.DirectorySeparatorChar + "*", out findData);
                }

                if (findHandle == INVALID_HANDLE_VALUE) return true;

                bool empty = true;
                int skipCnt = 2; // for skip root and parent dirs without need to compare strings. for perfomance
                bool letSkipCnt = true; // need to skip calculate skipCnt-- after skipCnt will be zero. for perfomance
                bool findHandleControl = false;
                //bool foundAny = false;
                do
                {
                    bool isDir = IsDir(findData.dwFileAttributes);
                    if ((isSubDir && letSkipCnt && (letSkipCnt = skipCnt-- > 0)) // replace of 2 checks below for . and ..
                        || (!isSubDir && (findData.cFileName == "." /*root dir*/ || findData.cFileName == ".." /*parent dir*/))
                        || findData.cFileName.ContainsAnyFrom(keywordsOfIgnoringNames)
                        || (isDir && searchForFiles && !searchForDirs && !recursive) // skip dir when need to find files
                        || (searchForDirs && !searchForFiles && !isDir) // skip file when need to find dirs
                        || (recursive && searchForFiles && isDir && !searchForDirs && IsNullOrEmptyDirectory(path + Path.DirectorySeparatorChar + findData.cFileName, mask, keywordsOfIgnoringNames, recursive, isSubDir: true, preciseMask: preciseMask)) // recursive and subfolder is empty. only for file search
                        || (!preciseMask && !defaultMask && recursive && searchForFiles && !isDir && !searchForDirs) // wait for possible dir to continue search in it when recursive search
                        || (preciseMask && ((searchForFiles && !isDir) || (searchForDirs && isDir)) && findData.cFileName != mask) // skip when mask not equals file/dir name
                        )
                    {
                        //if (!foundAny && defaultMask && ((searchForFiles && !isDir) || (searchForDirs && isDir)))
                        //{
                        //    foundAny = true;
                        //}

                        continue;
                    }
                    empty = false;
                } while (empty && (findHandleControl = FindNextFileW(findHandle, out findData)));

                //if (foundAny) // was found any
                //{
                //    return false;
                //}
                return empty;
            }
            finally
            {
                FindClose(findHandle); // will unlock dir
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
            if (exclusions != null && inputString.Length > 0)
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
