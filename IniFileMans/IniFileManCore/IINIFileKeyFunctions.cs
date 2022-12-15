using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IniFileManCore
{
    /// <summary>
    /// Base INI key functions
    /// </summary>
    public interface IINIFileKeyFunctions
    {
        /// <summary>
        /// Read value from selected key
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetKey(string section, string key);
        /// <summary>
        /// Set selected key's value
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetKey(string section, string key, string value);
        /// <summary>
        /// Delete selected key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        void DeleteKey(string key, string section = null);
        /// <summary>
        /// True when key exist
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        bool KeyExists(string key, string section = null);
    }
}
