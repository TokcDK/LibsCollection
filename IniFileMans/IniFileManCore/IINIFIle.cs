using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IniFileManCore
{
    /// <summary>
    /// Base INI function
    /// </summary>
    public interface IINIFile : IINIFileKeyFunctions, IINIFileSectionFunctions, IINIFileSectionExtraFunctions
    {
        /// <summary>
        /// Ini path fileinfo
        /// </summary>
        FileInfo INIPath { get; }
        /// <summary>
        /// Will write ini file
        /// </summary>
        void LoadFile();
        /// <summary>
        /// Will write ini file
        /// </summary>
        void WriteFile();
    }
}
