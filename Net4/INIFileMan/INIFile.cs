using IniParser;
using IniParser.Model;
using IniParser.Model.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace INIFileMan
{
    public class INIFile
    {
        //some info whic was used for very first variant: https://habr.com/ru/post/271483/

        private readonly FileInfo _iniPath; //Имя ini файла.
        private readonly FileIniDataParser _INIParser;

        /// <summary>
        /// Ini path fileinfo
        /// </summary>
        public FileInfo INIPath { get => _iniPath; }

        /// <summary>
        /// Configuration of ini file parser
        /// </summary>
        public IniParserConfiguration Configuration;
        public readonly IniData INIData;

        /// <summary>
        /// Load and manage ini file's content
        /// </summary>
        /// <param name="iniPath">Path to ini file</param>
        /// <param name="forceCreate">Force create the ini file if missing</param>
        public INIFile(string iniPath, bool forceCreate = false)
        {
            if (!File.Exists(iniPath) 
                && !forceCreate) return;

            if (forceCreate && !File.Exists(iniPath))
                File.WriteAllText(iniPath, "");

            this._iniPath = new FileInfo(iniPath);
            _INIParser = new FileIniDataParser();
            Configuration = _INIParser.Parser.Configuration;
            Configuration.AssigmentSpacer = ""; // no spaces before and after keyvalue splitter char

            INIData = _INIParser.ReadFile(this._iniPath.FullName, new UTF8Encoding(false));
        }

        /// <summary>
        /// Read value from selected key
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetKey(string section, string key)
        {
            if (INIData == null) return string.Empty;

            if (string.IsNullOrEmpty(section)) 
                return INIData.Global[key];

            if (!SectionExists(section))
                return string.Empty;

            return INIData[section][key];
        }

        /// <summary>
        /// Set selected key's value
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="doWriteFile"></param>
        public void SetKey(string section, string key, string value, bool doWriteFile = true)
        {
            if (INIData == null) return;

            bool changed = false;
            if (string.IsNullOrEmpty(section))
            {
                if (!INIData.Global.ContainsKey(key) || INIData.Global[key] != value)
                {
                    INIData.Global[key] = value;
                    changed = true;
                }
            }
            else
            {
                if (!SectionExists(section)) 
                    INIData.Sections.AddSection(section);

                if (!INIData[section].ContainsKey(key) 
                    || INIData[section][key] != value)
                {
                    INIData[section][key] = value;
                    changed = true;
                }
            }

            WriteFile(changed && doWriteFile);
        }

        /// <summary>
        /// Set selected key's value
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="doWriteFile"></param>
        public void SetKey(string section, string key, string value, string comment, bool doWriteFile = true)
        {
            if (INIData == null) return;

            bool changed = false;
            if (string.IsNullOrEmpty(section))
            {
                if (!INIData.Global.ContainsKey(key) || INIData.Global[key] != value)
                {
                    INIData.Global[key] = value;
                    SetComment(section, key, comment);
                    changed = true;
                }
            }
            else
            {
                if (!SectionExists(section))
                {
                    INIData.Sections.AddSection(section);
                }

                if (!INIData[section].ContainsKey(key) 
                    || INIData[section][key] != value)
                {
                    INIData[section][key] = value;
                    SetComment(section, key, comment);
                    changed = true;
                }
            }

            WriteFile(changed && doWriteFile);
        }

        /// <summary>
        /// Delete selected key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <param name="doWriteFile"></param>
        public void DeleteKey(string key, string section = null, bool doWriteFile = true)
        {
            if (INIData == null) return;

            bool changed = false;
            if (string.IsNullOrEmpty(section))
            {
                INIData.Global.RemoveKey(key);
                changed = true;
            }
            else
            {
                if (!SectionExists(section)) return;

                INIData[section].RemoveKey(key);
                changed = true;
            }

            WriteFile(changed && doWriteFile);
        }

        /// <summary>
        /// True when key exist
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool KeyExists(string key, string section = null)
        {
            if (INIData == null)  return false;

            if (string.IsNullOrEmpty(section)) 
                return INIData.Global.ContainsKey(key);

            return SectionExists(section) 
                && INIData[section].ContainsKey(key);
        }

        /// <summary>
        /// Delete selected secion with all keys in it
        /// </summary>
        /// <param name="section"></param>
        /// <param name="doWriteFile"></param>
        public void DeleteSection(string section/* = null*/, bool doWriteFile = true)
        {
            if (INIData == null) return;
            if (!SectionExists(section)) return;

            INIData.Sections.RemoveSection(section);
            WriteFile(doWriteFile);
        }

        /// <summary>
        /// Will read section's values to string array
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> GetSectionKeyValuePairs(string section)
        {
            if (INIData == null) yield break;

            if (string.IsNullOrEmpty(section) 
                || !SectionExists(section)) yield break;

            foreach (var keyData in INIData[section])
            {
                if (string.IsNullOrEmpty(keyData.KeyName)) continue;

                yield return new KeyValuePair<string, string>
                (
                    keyData.KeyName,
                    keyData.Value
                );
            }
        }

        /// <summary>
        /// Checks if <paramref name="section"/> is exists
        /// </summary>
        /// <param name="section"></param>
        /// <returns>True when <paramref name="section"/> is exists else False</returns>
        public bool SectionExists(string section)
        {
            return !string.IsNullOrEmpty(section) && INIData.Sections.ContainsSection(section);
        }

        /// <summary>
        /// Will read section's values or keys as strings
        /// </summary>
        /// <param name="section"></param>
        /// <param name="getValueElseKey">returns keyvalue when true or keyname when false</param>
        /// <returns></returns>
        public IEnumerable<string> GetSectionValues(string section, bool getValueElseKey = true)
        {
            if (INIData == null) yield break;

            if (string.IsNullOrEmpty(section) || !SectionExists(section)) yield break;

            foreach (var keyData in INIData[section])
            {
                if (string.IsNullOrEmpty(keyData?.KeyName)) continue;

                yield return getValueElseKey ? keyData.Value : keyData.KeyName;
            }
        }

        /// <summary>
        /// Enumerate sections of the ini
        /// </summary>
        /// <returns>section names</returns>
        public IEnumerable<string> EnumerateSectionNames()
        {
            if (INIData == null) yield break;

            foreach(var section in INIData.Sections) 
                yield return section.SectionName;
        }

        /// <summary>
        /// Enumerate section key names
        /// </summary>
        /// <param name="section"></param>
        /// <returns>section key names</returns>
        public IEnumerable<string> EnumerateSectionKeyNames(string section)
        {
            if (INIData == null) yield break;
            if (!SectionExists(section)) yield break;

            foreach(var key in INIData.Sections.GetSectionData(section).Keys) 
                yield return key.KeyName;
        }

        /// <summary>
        /// Will read section's values to string dictionary
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetSectionValuesToDictionary(string section)
        {
            var ret = new Dictionary<string, string>();

            foreach (var val in GetSectionKeyValuePairs(section)) 
                ret.Add(val.Key, val.Value);

            return ret;
        }

        /// <summary>
        /// Change section values to selected section like "0='value0'" "1='value1'"...
        /// </summary>
        /// <param name="section"></param>
        /// <param name="values"></param>
        /// <param name="cleanSectionBeforeWrite"></param>
        /// <param name="doWriteFile"></param>
        public void SetArrayToSectionValues(string section, string[] values, bool cleanSectionBeforeWrite = true, bool doWriteFile = true)
        {
            if (INIData == null) return;
            if (string.IsNullOrEmpty(section) ) return;
            if (values == null ) return;
            var valuesLength = values.Length;
            if (valuesLength == 0) return;

            if (cleanSectionBeforeWrite) ClearSection(section);

            for (int i = 0; i < valuesLength; i++) 
                SetKey(section, i + "", values[i]);

            WriteFile(doWriteFile);
        }

        /// <summary>
        /// Will write ini file
        /// </summary>
        /// <param name="doWriteFile">Command to write ini</param>
        /// <param name="ActionWasExecuted">Additional command to write ini</param>
        public void WriteFile(bool doWriteFile = true, bool utf8NoBom = true)
        {
            if (!doWriteFile) return;

            //https://stackoverflow.com/questions/2502990/create-text-file-without-bom
            _INIParser.WriteFile(_iniPath.FullName, INIData, new UTF8Encoding(!utf8NoBom));
        }

        /// <summary>
        /// Will delet section's values
        /// </summary>
        /// <param name="section"></param>
        /// <param name="doWriteFile"></param>
        public void ClearSection(string section/* = null*/, bool doWriteFile = true)
        {
            if (INIData == null) return;
            if (!SectionExistsAndNotEmpty(section)) return;

            INIData.Sections[section].RemoveAllKeys();
            WriteFile(doWriteFile);
        }

        /// <summary>
        /// True when selected section is exists and have any keys
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool SectionExistsAndNotEmpty(string section = null)
        {
            return SectionExists(section) && INIData[section].Count > 0;
        }

        /// <summary>
        /// Set commentary for the comment
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="comment"></param>
        /// <param name="doWriteFile"></param>
        public void SetComment(string section, string key, string comment, bool doWriteFile = true)
        {
            var keyData = new KeyData(key);
            if(KeyExists(key, section)) 
                keyData.Value = GetKey(section, key);

            keyData.Comments.Clear();
            keyData.Comments.Add(comment);
            INIData.Sections[section].SetKeyData(keyData);

            WriteFile(doWriteFile);
        }
    }
}
