using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

// 2025.05.01

namespace Common
{
    /// <summary>
    /// Handles loading, saving, and managing configuration settings from an XML file.
    /// Implements INotifyPropertyChanged for binding scenarios in WPF.
    /// </summary>
    public class XmlConfigurationHandler : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the XmlConfigurationHandler class.
        /// </summary>
        /// <param name="FilePath">The full path to the configuration XML file.</param>
        public XmlConfigurationHandler(string FilePath)
        {
            this._FilePath = FilePath;
            Settings = new ExpandoObject();
        }

        private string _FilePath = null;
        /// <summary>
        /// Gets the full path to the configuration XML file.
        /// </summary>
        public string FilePath
        {
            get => _FilePath;
        }

        /// <summary>
        /// Gets the current settings as a dictionary of key-value pairs.
        /// </summary>
        /// <returns>A dictionary containing the settings.</returns>
        public Dictionary<string, string> GetSettingDictionary()
        {
            var expandoDict = (IDictionary<string, object>)Settings;
            if (expandoDict == null)
            {
                return new Dictionary<string, string>();
            }

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var item in expandoDict)
            {
                dictionary.Add(item.Key, item.Value.ToString());
            }

            return dictionary;
        }

        private ExpandoObject _Settings = null;
        /// <summary>
        /// Gets or sets the dynamic collection of settings loaded from or to be saved to the XML file.
        /// </summary>
        public ExpandoObject Settings
        {
            get => _Settings;
            set
            {
                if (_Settings != value)
                {
                    _Settings = value;
                    OnPropertyChanged(nameof(Settings));
                }
            }
        }

        /// <summary>
        /// Removes a setting by its key.
        /// </summary>
        /// <param name="Key">The key of the setting to remove.</param>
        public void RemoveSetting(string Key)
        {
            var expandoDict = (IDictionary<string, object>)Settings;

            if (expandoDict.ContainsKey(Key))
            {
                expandoDict.Remove(Key);
            }
        }

        /// <summary>
        /// Checks if a setting with the specified key exists.
        /// </summary>
        /// <param name="Key">The key to check for.</param>
        /// <returns>True if the setting exists, otherwise false.</returns>
        public bool ContainsSetting(string Key)
        {
            var expandoDict = (IDictionary<string, object>)Settings;

            return expandoDict.ContainsKey(Key);
        }

        /// <summary>
        /// Adds a new setting with a default value if the key does not exist.
        /// If the key exists, it updates the existing value with the default value.
        /// </summary>
        /// <param name="Key">The key of the setting.</param>
        /// <param name="DefaultValue">The default value to add or set.</param>
        public void AddSetting(string Key, string DefaultValue)
        {
            var expandoDict = (IDictionary<string, object>)Settings;

            if (expandoDict.ContainsKey(Key))
            {
                //有此參數就寫入
                //If this parameter exists, write to it
                WriteString(Key, DefaultValue);
                return;
            }

            if (!expandoDict.ContainsKey(Key))
            {
                //無此參數就新增
                //If this parameter does not exist, add it
                expandoDict.Add(Key, DefaultValue);
                return;
            }
        }

        #region Read

        /// <summary>
        /// Loads the configuration settings from the XML file specified by FilePath.
        /// Creates the file with current settings if it doesn't exist.
        /// Settings from the file overwrite existing settings with the same key.
        /// </summary>
        public void LoadFile()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                return;
            }

            if (!System.IO.File.Exists(FilePath))
            {
                //無此檔案就建立檔案(之前必須先新增設定)
                //If the file does not exist, create the file (settings must have been added previously)
                SaveFile();
                return;
            }

            try
            {
                var doc = XDocument.Load(FilePath);
                XElement root = doc.Root;
                if (root != null)
                {
                    foreach (var item in root.Elements())
                    {
                        string name = item.Name.ToString(); // Updated variable name
                        string value = item.Value.ToString(); // Updated variable name
                        WriteString(name, value);
                    }
                }
            }
            catch (Exception ex)
            {
                // Consider adding logging here for the exception (ex)
            }
        }

        /// <summary>
        /// Reads a setting as a string.
        /// </summary>
        /// <param name="Key">The key of the setting to read.</param>
        /// <returns>The string value of the setting, or null if the key does not exist.</returns>
        public string ReadString(string Key)
        {
            var expandoDict = (IDictionary<string, object>)Settings;

            if (expandoDict.ContainsKey(Key))
            {
                return expandoDict[Key].ToString();
            }

            //沒這個參數的話就傳null
            //If there is no such parameter, return null
            return null;
        }

        /// <summary>
        /// Reads a setting as a double.
        /// </summary>
        /// <param name="Key">The key of the setting to read.</param>
        /// <param name="digits">The number of decimal places to round to. -1 for no rounding.</param>
        /// <returns>The double value of the setting, rounded if specified, or double.NaN if the key does not exist or parsing fails.</returns>
        public double ReadDouble(string Key, int digits = -1)
        {
            // 小數位數 -1表示不四捨五入
            // Decimal places -1 means no rounding
            // 小數位數 0表示不顯示小數點
            // Decimal places 0 means no decimal point displayed
            // 小數位數 1表示顯示一位小數
            // Decimal places 1 means display one decimal place

            string parameterValueString = ReadString(Key); // Updated variable name

            double d = double.NaN;

            if (parameterValueString == null || !double.TryParse(parameterValueString, out d))
            {
                // d is already double.NaN
                return double.NaN;
            }

            if (digits < 0)
            {
                //不四捨五入
                //No rounding
                return d;
            }

            //四捨五入
            //Rounding
            return Math.Round(d, digits);
        }

        /// <summary>
        /// Reads a setting as an integer.
        /// </summary>
        /// <param name="Key">The key of the setting to read.</param>
        /// <returns>The integer value of the setting, or 0 if the key does not exist or parsing fails.</returns>
        public int ReadInt(string Key)
        {
            string parameterValueString = ReadString(Key); // Updated variable name

            int i = 0;

            if (parameterValueString == null || !int.TryParse(parameterValueString, out i))
            {
                // i is already 0
                return 0;
            }

            return i;
        }

        /// <summary>
        /// Reads a setting as a boolean. Accepts "1", "yes", "true" (case-insensitive) as true.
        /// </summary>
        /// <param name="Key">The key of the setting to read.</param>
        /// <returns>The boolean value of the setting, or false if the key does not exist or parsing fails.</returns>
        public bool ReadBoolean(string Key)
        {
            bool MyBoolean = false;

            string parameterValueString = ReadString(Key); // Updated variable name
            if (parameterValueString == null) return false; // Handle null case

            parameterValueString = parameterValueString.Trim().ToLower();

            if (parameterValueString == "1" || parameterValueString == "yes" || parameterValueString == "true")
            {
                MyBoolean = true;
            }

            return MyBoolean;
        }

        #endregion

        #region Write

        /// <summary>
        /// Saves the current settings to the XML file specified by FilePath.
        /// Overwrites the file if it already exists.
        /// </summary>
        public void SaveFile()
        {
            try
            {
                XElement xRoot = new XElement("Settings");
                var expandoDict = (IDictionary<string, object>)Settings;
                foreach (var item in expandoDict)
                {
                    XElement xItem = new XElement(item.Key, item.Value.ToString());
                    xRoot.Add(xItem);
                }
                xRoot.Save(FilePath);
            }
            catch (Exception ex)
            {
                // Consider adding logging here for the exception (ex)
            }
        }

        /// <summary>
        /// Writes a string value to an existing setting.
        /// Does nothing if the key does not exist.
        /// </summary>
        /// <param name="Key">The key of the setting.</param>
        /// <param name="Value">The string value to write.</param>
        public void WriteString(string Key, string Value)
        {
            var expandoDict = (IDictionary<string, object>)Settings;

            if (!expandoDict.ContainsKey(Key))
            {
                //沒這個參數的話就禁止寫入
                //If there is no such parameter, writing is prohibited
                return;
            }

            expandoDict[Key] = Value;
        }

        /// <summary>
        /// Writes a double value to an existing setting, optionally rounding it before writing.
        /// </summary>
        /// <param name="Key">The key of the setting.</param>
        /// <param name="Value">The double value to write.</param>
        /// <param name="digits">The number of decimal places to round to before writing. -1 for no rounding.</param>
        public void WriteDouble(string Key, double Value, int digits = -1)
        {
            string formatString = string.Empty;
            if (digits > -1)
            {
                formatString = "0.";

                for (int i = 0; i < digits; i++)
                {
                    formatString += "#";
                }

                //(3.1230000).ToString("0.####");    結果是 3.123
            }

            string stringToWrite = Value.ToString(formatString);

            WriteString(Key, stringToWrite);
        }

        /// <summary>
        /// Writes an integer value to an existing setting.
        /// </summary>
        /// <param name="Key">The key of the setting.</param>
        /// <param name="Value">The integer value to write.</param>
        public void WriteInt(string Key, int Value)
        {
            string stringToWrite = Value.ToString();
            WriteString(Key, stringToWrite);
        }

        /// <summary>
        /// Writes a boolean value to an existing setting ("1" for true, "0" for false).
        /// </summary>
        /// <param name="Key">The key of the setting.</param>
        /// <param name="Value">The boolean value to write.</param>
        public void WriteBoolean(string Key, bool Value)
        {
            string stringToWrite = "0";

            if (Value)
            {
                stringToWrite = "1";
            }

            WriteString(Key, stringToWrite);
        }

        #endregion

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}