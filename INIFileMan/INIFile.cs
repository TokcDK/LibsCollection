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

        private readonly string _iniPath; //Имя ini файла.
        private readonly FileIniDataParser _INIParser;
        /// <summary>
        /// Configuration of ini file parser
        /// </summary>
        public IniParserConfiguration Configuration;
        private readonly IniData _INIData;

        //old variant code
        //[DllImport("kernel32", CharSet = CharSet.Unicode)] // Подключаем kernel32.dll и описываем его функцию WritePrivateProfilesString
        //static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        //[DllImport("kernel32", CharSet = CharSet.Unicode)] // Еще раз подключаем kernel32.dll, а теперь описываем функцию GetPrivateProfileString
        //static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        //public IniData GetINIData()
        //{
        //    return INIData;
        //}

        /// <summary>
        /// Load and manage ini file's content
        /// </summary>
        /// <param name="iniPath">Path to ini file</param>
        /// <param name="forceCreate">Force create the ini file if missing</param>
        public INIFile(string iniPath, bool forceCreate = false)
        {
            //if (!File.Exists(Path))
            //{
            //    File.WriteAllText(Path, string.Empty);
            //}
            if (File.Exists(iniPath) || forceCreate)
            {
                if(forceCreate && !File.Exists(iniPath))
                {
                    File.WriteAllText(iniPath, "");
                }

                this._iniPath = iniPath;
                _INIParser = new FileIniDataParser();
                Configuration = _INIParser.Parser.Configuration;
                Configuration.AssigmentSpacer = ""; // no spaces before and after keyvalue splitter char

                _INIData = _INIParser.ReadFile(this._iniPath, new UTF8Encoding(false));
            }
            //else
            //{
            //    throw new FileNotFoundException();
            //}
        }

        /// <summary>
        /// Read value from selected key
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetKey(string section, string key)
        {
            if (_INIData == null)
                return string.Empty;

            if (string.IsNullOrEmpty(section))
            {
                return _INIData.Global[key];
            }
            else
            {
                if (!SectionExists(section))
                {
                    return string.Empty;
                }
                return _INIData[section][key];
            }

            //var ini = ExIni.IniFile.FromFile(Path);
            //var section = ini.GetSection(Section);
            //if (section == null)
            //{
            //    return string.Empty;
            //}
            //else
            //{
            //    var key = section.GetKey(Key);

            //    if (key != null)//ExIni не умеет читать ключи с \\ в имени
            //    {
            //        return key.Value;
            //    }
            //    else
            //    {
            //        var RetVal = new StringBuilder(4096);
            //        GetPrivateProfileString(Section, Key, "", RetVal, 4096, Path);//Это почему-то не может прочитать ключи из секций с определенными названиями
            //        return RetVal.ToString();
            //    }
            //}

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
            if (_INIData == null)
                return;

            bool changed = false;
            if (string.IsNullOrEmpty(section))
            {
                if (!_INIData.Global.ContainsKey(key) || _INIData.Global[key] != value)
                {
                    _INIData.Global[key] = value;
                    changed = true;
                }
            }
            else
            {
                if (!SectionExists(section))
                {
                    _INIData.Sections.AddSection(section);
                }

                if (!_INIData[section].ContainsKey(key) || _INIData[section][key] != value)
                {
                    _INIData[section][key] = value;
                    changed = true;
                }
            }

            WriteFile(changed && doWriteFile);
            //if (!ManageStrings.IsStringAContainsStringB(Key, "\\"))
            //{
            //    var ini = ExIni.IniFile.FromFile(Path);
            //    var section = ini.GetSection(Section);
            //    if (section != null)
            //    {
            //        if (section.HasKey(Key))//ExIni не умеет читать ключи с \\ в имени
            //        {
            //            section.GetKey(Key).Value = Value;
            //            ini.Save(Path);
            //            return;
            //        }
            //        else
            //        {
            //            section.CreateKey(Key);
            //            section.GetKey(Key).Value = Value;
            //            ini.Save(Path);
            //            return;
            //        }
            //    }
            //}
            //WritePrivateProfileString(Section, Key, Value, Path);
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
            if (_INIData == null)
                return;

            bool changed = false;
            if (string.IsNullOrEmpty(section))
            {
                if (!_INIData.Global.ContainsKey(key) || _INIData.Global[key] != value)
                {
                    _INIData.Global[key] = value;
                    SetComment(section, key, comment);
                    changed = true;
                }
            }
            else
            {
                if (!SectionExists(section))
                {
                    _INIData.Sections.AddSection(section);
                }

                if (!_INIData[section].ContainsKey(key) || _INIData[section][key] != value)
                {
                    _INIData[section][key] = value;
                    SetComment(section, key, comment);
                    changed = true;
                }
            }

            WriteFile(changed && doWriteFile);
            //if (!ManageStrings.IsStringAContainsStringB(Key, "\\"))
            //{
            //    var ini = ExIni.IniFile.FromFile(Path);
            //    var section = ini.GetSection(Section);
            //    if (section != null)
            //    {
            //        if (section.HasKey(Key))//ExIni не умеет читать ключи с \\ в имени
            //        {
            //            section.GetKey(Key).Value = Value;
            //            ini.Save(Path);
            //            return;
            //        }
            //        else
            //        {
            //            section.CreateKey(Key);
            //            section.GetKey(Key).Value = Value;
            //            ini.Save(Path);
            //            return;
            //        }
            //    }
            //}
            //WritePrivateProfileString(Section, Key, Value, Path);
        }

        /// <summary>
        /// Delete selected key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <param name="doWriteFile"></param>
        public void DeleteKey(string key, string section = null, bool doWriteFile = true)
        {
            if (_INIData == null)
                return;

            bool changed = false;
            if (string.IsNullOrEmpty(section))
            {
                _INIData.Global.RemoveKey(key);
                changed = true;
            }
            else
            {
                if (SectionExists(section))
                {
                    _INIData[section].RemoveKey(key);
                    changed = true;
                }
            }

            WriteFile(changed && doWriteFile);
            //var ini = ExIni.IniFile.FromFile(Path);
            //var section = ini.GetSection(Section);
            //if (section != null)
            //{
            //    if (section.HasKey(Key) && !ManageStrings.IsStringAContainsStringB(Key,"\\"))//ExIni не умеет читать ключи с \\ в имени
            //    {
            //        section.DeleteKey(Key);
            //        ini.Save(Path);
            //    }
            //    else
            //    {
            //        WriteINI(Section, Key, null);
            //    }
            //}
        }

        /// <summary>
        /// True when key exist
        /// </summary>
        /// <param name="key"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool KeyExists(string key, string section = null)
        {
            if (_INIData == null)
                return false;

            if (string.IsNullOrEmpty(section))
            {
                return _INIData.Global.ContainsKey(key);
            }
            else
            {
                if (!SectionExists(section))
                {
                    return false;
                }
                return _INIData[section].ContainsKey(key);
            }
            //var iniSection = ExIni.IniFile.FromFile(Path).GetSection(Section);
            //if (iniSection == null)
            //{
            //    return false;
            //}
            //else
            //{
            //    return iniSection.HasKey(Key);
            //}
            //MessageBox.Show("key length="+ ReadINI(Section, Key).Length);
            //return ReadINI(Section, Key).Length > 0;
        }

        /// <summary>
        /// Delete selected secion with all keys in it
        /// </summary>
        /// <param name="section"></param>
        /// <param name="doWriteFile"></param>
        public void DeleteSection(string section/* = null*/, bool doWriteFile = true)
        {
            if (_INIData == null)
                return;

            if (SectionExists(section))
            {
                _INIData.Sections.RemoveSection(section);
                WriteFile(doWriteFile);
            }

            //var ini = ExIni.IniFile.FromFile(Path);
            //if(Section!=null && ini.HasSection(Section))
            //{
            //    ExIni.IniFile.FromFile(Path).DeleteSection(Section);
            //    ini.Save(Path);
            //}
            //WriteINI(Section, null, null);
        }

        /// <summary>
        /// Will read section's values to string array
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> GetSectionKeyValuePairs(string section)
        {
            if (_INIData == null)
                yield break;

            if (string.IsNullOrEmpty(section) || !SectionExists(section))
            {
                yield break;
            }
            else
            {
                foreach (var keyData in _INIData[section])
                {
                    if (!string.IsNullOrEmpty(keyData.KeyName))
                    {
                        yield return new KeyValuePair<string, string>
                        (
                            keyData.KeyName,
                            keyData.Value
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Checks if <paramref name="section"/> is exists
        /// </summary>
        /// <param name="section"></param>
        /// <returns>True when <paramref name="section"/> is exists else False</returns>
        public bool SectionExists(string section)
        {
            return !string.IsNullOrEmpty(section) && _INIData.Sections.ContainsSection(section);
        }

        /// <summary>
        /// Will read section's values or keys as strings
        /// </summary>
        /// <param name="section"></param>
        /// <param name="getValueElseKey">returns keyvalue when true or keyname when false</param>
        /// <returns></returns>
        public IEnumerable<string> GetSectionValues(string section, bool getValueElseKey = true)
        {
            if (_INIData == null)
                yield break;

            if (string.IsNullOrEmpty(section) || !SectionExists(section))
            {
                yield break;
            }
            else
            {
                foreach (var keyData in _INIData[section])
                {
                    if (!string.IsNullOrEmpty(keyData?.KeyName))
                    {
                        if (getValueElseKey)
                        {
                            yield return keyData.Value;
                        }
                        else
                        {
                            yield return keyData.KeyName;
                        }
                    }
                }
            }
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
            {
                ret.Add(val.Key, val.Value);
            }

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
            if (_INIData == null || string.IsNullOrEmpty(section) || values == null || values.Length == 0)
                return;

            if (cleanSectionBeforeWrite)
            {
                ClearSection(section);
            }

            for (int i = 0; i < values.Length; i++)
            {
                SetKey(section, i + "", values[i]);
            }
            WriteFile(doWriteFile);
        }

        /// <summary>
        /// Will write ini file
        /// </summary>
        /// <param name="doWriteFile">Command to write ini</param>
        /// <param name="ActionWasExecuted">Additional command to write ini</param>
        public void WriteFile(bool doWriteFile = true)
        {
            if (doWriteFile)
            {
                //if (Path.GetFileName(iniPath) == "AutoTranslatorConfig.ini")
                //{
                //    INIData.Configuration.AssigmentSpacer = string.Empty;
                //}
                //https://stackoverflow.com/questions/2502990/create-text-file-without-bom
                _INIParser.WriteFile(_iniPath, _INIData, new UTF8Encoding(false));
            }
        }

        /// <summary>
        /// Will delet section's values
        /// </summary>
        /// <param name="section"></param>
        /// <param name="doWriteFile"></param>
        public void ClearSection(string section/* = null*/, bool doWriteFile = true)
        {
            if (_INIData == null)
                return;

            if (SectionExistsAndNotEmpty(section))
            {
                _INIData.Sections[section].RemoveAllKeys();
                WriteFile(doWriteFile);
            }
            //var ini = ExIni.IniFile.FromFile(Path);
            //if(Section!=null && ini.HasSection(Section))
            //{
            //    ExIni.IniFile.FromFile(Path).DeleteSection(Section);
            //    ini.Save(Path);
            //}
            //WriteINI(Section, null, null);
        }

        /// <summary>
        /// True when selected section is exists and have any keys
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool SectionExistsAndNotEmpty(string section = null)
        {
            return SectionExists(section) && _INIData[section].Count > 0;
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
            {
                keyData.Value = GetKey(section, key);
            }

            keyData.Comments.Clear();
            keyData.Comments.Add(comment);
            _INIData.Sections[section].SetKeyData(keyData);

            WriteFile(doWriteFile);
        }


    }
}
