using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IniFileManCore
{
    /// <summary>
    /// Base INI section functions
    /// </summary>
    public interface IINIFileSectionFunctions
    {
        /// <summary>
        /// Checks if <paramref name="section"/> is exists
        /// </summary>
        /// <param name="section"></param>
        /// <returns>True when <paramref name="section"/> is exists else False</returns>
        bool SectionExists(string section);
        /// <summary>
        /// Delete selected secion with all keys in it
        /// </summary>
        /// <param name="section"></param>
        void DeleteSection(string section);
    }

    /// <summary>
    /// Base INI section functions
    /// </summary>
    public interface IINIFileSectionExtraFunctions
    {
        /// <summary>
        /// Will delete section's values
        /// </summary>
        /// <param name="section"></param>
        /// <param name="doWriteFile"></param>
        void ClearSection(string section/* = null*/, bool doWriteFile = true);

        /// <summary>
        /// True when selected section is exists and have any keys
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        bool SectionExistsAndNotEmpty(string section);
    }
}
