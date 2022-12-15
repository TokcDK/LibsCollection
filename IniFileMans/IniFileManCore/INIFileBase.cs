using System.Collections.Generic;
using System.IO;
using System.Text;
using IniFileManCore;

namespace INIFileMan
{
    /// <summary>
    /// Base for ini file parsers
    /// </summary>
    public abstract class INIFileBase : IINIFile
    {
        /// <summary>
        /// Ini path fileinfo
        /// </summary>
        public abstract FileInfo INIPath { get; }
        /// <summary>
        /// Will load ini file
        /// </summary>
        public abstract void LoadFile();
        /// <summary>
        /// Will write ini file
        /// </summary>
        public abstract void WriteFile();

        /// <summary>
        /// Read value from selected key
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract string GetKey(string section, string key);

        /// <summary>
        /// Set selected key's value
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void SetKey(string section, string key, string value);

        /// <summary>
        /// Delete selected key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        public abstract void DeleteKey(string key, string section = null);

        /// <summary>
        /// True when key exist
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public abstract bool KeyExists(string key, string section = null);

        /// <summary>
        /// Delete selected secion with all keys in it
        /// </summary>
        /// <param name="section"></param>
        public abstract void DeleteSection(string section);

        /// <summary>
        /// Will read section's values to string array
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public abstract IEnumerable<KeyValuePair<string, string>> GetSectionKeyValuePairs(string section);

        /// <summary>
        /// Checks if <paramref name="section"/> is exists
        /// </summary>
        /// <param name="section"></param>
        /// <returns>True when <paramref name="section"/> is exists else False</returns>
        public abstract bool SectionExists(string section);

        /// <summary>
        /// Enumerate sections of the ini
        /// </summary>
        /// <returns>section names</returns>
        public abstract IEnumerable<string> EnumerateSectionNames();

        /// <summary>
        /// Enumerate section key names
        /// </summary>
        /// <param name="section"></param>
        /// <returns>section key names</returns>
        public abstract IEnumerable<string> EnumerateSectionKeyNames(string section);

        /// <summary>
        /// Will read section's values of every key
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public abstract IEnumerable<string> EnumerateSectionValues(string section);

        /// <summary>
        /// Will read section's values to string dictionary
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public abstract Dictionary<string, string> GetSectionValuesToDictionary(string section);

        /// <summary>
        /// Change section values to selected section like "0='value0'" "1='value1'"...
        /// </summary>
        /// <param name="section"></param>
        /// <param name="values"></param>
        /// <param name="cleanSectionBeforeWrite"></param>
        public abstract void SetArrayToSectionValues(string section, string[] values, bool cleanSectionBeforeWrite = true);

        /// <summary>
        /// Write ini file with selected encoding
        /// </summary>
        /// <param name="encoding"></param>
        public abstract void WriteFileWithEncoding(Encoding encoding);

        /// <summary>
        /// Will delet section's values
        /// </summary>
        /// <param name="section"></param>
        /// <param name="doWriteFile"></param>
        public abstract void ClearSection(string section/* = null*/, bool doWriteFile = true);

        /// <summary>
        /// True when selected section is exists and have any keys
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public abstract bool SectionExistsAndNotEmpty(string section);

        /// <summary>
        /// Set commentary for the comment
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="comment"></param>
        public abstract void SetComment(string section, string key, string comment);
    }
}
