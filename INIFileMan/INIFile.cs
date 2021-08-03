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

        private readonly string iniPath; //Имя ini файла.
        private readonly FileIniDataParser INIParser;
        /// <summary>
        /// Configuration of ini file parser
        /// </summary>
        public IniParserConfiguration Configuration;
        private readonly IniData INIData;
        bool ActionWasExecuted;

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
        /// <param name="IniPath"></param>
        public INIFile(string IniPath)
        {
            iniPath = IniPath;
            INIParser = new FileIniDataParser();
            Configuration = INIParser.Parser.Configuration;
            Configuration.AssigmentSpacer = ""; // no spaces before and after keyvalue splitter char
            //if (!File.Exists(Path))
            //{
            //    File.WriteAllText(Path, string.Empty);
            //}
            if (File.Exists(iniPath))
            {
                INIData = INIParser.ReadFile(iniPath, new UTF8Encoding(false));
            }
            //else
            //{
            //    throw new FileNotFoundException();
            //}
        }

        /// <summary>
        /// Read value from selected key
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string ReadKey(string Section, string Key)
        {
            if (INIData == null)
                return string.Empty;

            if (string.IsNullOrEmpty(Section))
            {
                return INIData.Global[Key];
            }
            else
            {
                if (!INIData.Sections.ContainsSection(Section))
                {
                    return string.Empty;
                }
                return INIData[Section][Key];
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
        /// Will read section's values to string array
        /// </summary>
        /// <param name="Section"></param>
        /// <returns></returns>
        public IEnumerable<string> ReadSectionValues(string Section)
        {
            if (INIData == null)
                yield break;

            if (string.IsNullOrEmpty(Section) || !INIData.Sections.ContainsSection(Section))
            {
                yield break;
            }
            else
            {
                using (var sectionKeys = INIData[Section].GetEnumerator())
                {

                    for (int i = 0; i < INIData[Section].Count; i++)
                    {
                        sectionKeys.MoveNext();

                        if (sectionKeys.Current.Value.Length > 0)
                        {
                            yield return sectionKeys.Current.Value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Will read section's values to string dictionary
        /// </summary>
        /// <param name="Section"></param>
        /// <returns></returns>
        public Dictionary<string, string> ReadSectionValuesToDictionary(string Section)
        {
            var ret = new Dictionary<string, string>();

            int i = 0;
            foreach (var val in ReadSectionValues(Section))
            {
                ret.Add(i + "", val);
            }

            return ret;
        }

        /// <summary>
        /// Change section values to selected section like "0='value0'" "1='value1'"...
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Values"></param>
        /// <param name="CleanSectionBeforeWrite"></param>
        /// <param name="DoSaveINI"></param>
        public void WriteArrayToSectionValues(string Section, string[] Values, bool CleanSectionBeforeWrite = true, bool DoSaveINI = true)
        {
            if (INIData == null || string.IsNullOrEmpty(Section) || Values == null || Values.Length == 0)
                return;

            if (CleanSectionBeforeWrite)
            {
                INIData[Section].RemoveAllKeys();
            }

            for (int i = 0; i < Values.Length; i++)
            {
                var v = Values[i];
                INIData[Section][i + ""] = v;
            }
            Write(DoSaveINI, ActionWasExecuted);
        }

        /// <summary>
        /// Set selected key's value
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <param name="DoSaveINI"></param>
        public void SetKey(string Section, string Key, string Value, bool DoSaveINI = true)
        {
            if (INIData == null)
                return;

            ActionWasExecuted = false;

            if (string.IsNullOrEmpty(Section))
            {
                if (INIData.Global[Key] != Value)
                {
                    INIData.Global[Key] = Value;
                    ActionWasExecuted = true;
                }
            }
            else
            {
                bool exists;
                if ((exists = INIData.Sections.ContainsSection(Section)) && INIData[Section][Key] != Value)
                {
                    if (!exists)
                    {
                        INIData.Sections.AddSection(Section);
                    }

                    INIData[Section][Key] = Value;
                    ActionWasExecuted = true;
                }
            }
            Write(DoSaveINI, ActionWasExecuted);
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
        /// Delet selected key
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Section"></param>
        /// <param name="DoSaveINI"></param>
        public void DeleteKey(string Key, string Section = null, bool DoSaveINI = true)
        {
            if (INIData == null)
                return;

            ActionWasExecuted = false;

            if (string.IsNullOrEmpty(Section))
            {
                INIData.Global.RemoveKey(Key);
                ActionWasExecuted = true;
            }
            else
            {
                if (INIData.Sections.ContainsSection(Section))
                {
                    INIData[Section].RemoveKey(Key);
                    ActionWasExecuted = true;
                }
            }
            Write(DoSaveINI, ActionWasExecuted);
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
        /// Delete selected secion with all keys in it
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="DoSaveINI"></param>
        public void DeleteSection(string Section/* = null*/, bool DoSaveINI = true)
        {
            if (INIData == null)
                return;

            ActionWasExecuted = false;

            if (INIData.Sections.ContainsSection(Section))
            {
                INIData.Sections.RemoveSection(Section);
                ActionWasExecuted = true;
            }

            Write(DoSaveINI, ActionWasExecuted);
            //var ini = ExIni.IniFile.FromFile(Path);
            //if(Section!=null && ini.HasSection(Section))
            //{
            //    ExIni.IniFile.FromFile(Path).DeleteSection(Section);
            //    ini.Save(Path);
            //}
            //WriteINI(Section, null, null);
        }

        /// <summary>
        /// Will write ini file
        /// </summary>
        /// <param name="DoSaveINI">Command to write ini</param>
        /// <param name="ActionWasExecuted">Additional command to write ini</param>
        public void Write(bool DoSaveINI = true, bool ActionWasExecuted = true)
        {
            if (DoSaveINI && ActionWasExecuted)
            {
                //if (Path.GetFileName(iniPath) == "AutoTranslatorConfig.ini")
                //{
                //    INIData.Configuration.AssigmentSpacer = string.Empty;
                //}
                //https://stackoverflow.com/questions/2502990/create-text-file-without-bom
                INIParser.WriteFile(iniPath, INIData, new UTF8Encoding(false));
            }
        }

        /// <summary>
        /// Will delet section's values
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="DoSaveINI"></param>
        public void ClearSection(string Section/* = null*/, bool DoSaveINI = true)
        {
            if (INIData == null)
                return;

            ActionWasExecuted = false;

            if (INIData.Sections.ContainsSection(Section))
            {
                INIData.Sections[Section].RemoveAllKeys();
                ActionWasExecuted = true;
            }
            Write(DoSaveINI, ActionWasExecuted);
            //var ini = ExIni.IniFile.FromFile(Path);
            //if(Section!=null && ini.HasSection(Section))
            //{
            //    ExIni.IniFile.FromFile(Path).DeleteSection(Section);
            //    ini.Save(Path);
            //}
            //WriteINI(Section, null, null);
        }

        /// <summary>
        /// True when key exist
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Section"></param>
        /// <returns></returns>
        public bool KeyExists(string Key, string Section = null)
        {
            if (INIData == null)
                return false;

            if (string.IsNullOrEmpty(Section))
            {
                return INIData.Global.ContainsKey(Key);
            }
            else
            {
                if (!INIData.Sections.ContainsSection(Section))
                {
                    return false;
                }
                return INIData[Section].ContainsKey(Key);
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
        /// True when selected section is exists and have any keys
        /// </summary>
        /// <param name="Section"></param>
        /// <returns></returns>
        public bool SectionExistsAndNotEmpty(string Section = null)
        {
            if (INIData == null)
                return false;

            if (string.IsNullOrEmpty(Section))
            {
                return false;
            }
            else
            {
                if (!INIData.Sections.ContainsSection(Section))
                {
                    return false;
                }
                return INIData[Section].Count > 0;
            }
        }
    }
}
