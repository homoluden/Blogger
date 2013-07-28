using Blogger.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using ServiceStack.Text;
using Blogger.Data.Localization;

namespace Blogger.Strings
{
    public sealed class StringsManagers
    {
        #region Constants
        public static readonly string NOT_AVAILABLE_STRING = "N/A";
        #endregion

        #region Fields
        CultureInfo _currCulture;
        Dictionary<string, string> _cache = new Dictionary<string, string>();
        #endregion


        #region Static Methods
        private static void LoadDictionary(StringsManagers manager, string filename)
        {
            var assm = Assembly.GetExecutingAssembly();

            var jsonString = string.Empty;
            using (var embeddedResource = assm.GetManifestResourceStream("Blogger.Strings.Assets." + filename))
            {
                using (var reader = new StreamReader(embeddedResource))
                {
                    jsonString = reader.ReadToEnd();
                }
            }

            var dictItems = jsonString.FromJson<DictionaryItem[]>();

            if (dictItems.Length == 0)
            {
                throw new FileNotFoundException("Dictionary Resource not found, empty or has wrong format.");
            }

            foreach (var item in dictItems)
            {
                manager._cache.Add(item.Key, item.Value);
            }
        }

        public static string GetSettingName(Setting settingNameEnum)
        {
            if (SettingNames._cache.Count() == 0)
            {
                LoadDictionary(SettingNames, "setting.names.json");
            }

            return SettingNames.GetString(settingNameEnum.ToString());
        }

        public static string GetSettingDefaultValue(Setting settingNameEnum)
        {
            if (SettingDefaults._cache.Count() == 0)
            {
                LoadDictionary(SettingDefaults, "setting.defs.json");
            }

            return SettingDefaults.GetString(settingNameEnum.ToString());
        }
        #endregion


        #region Public Methods
        public string GetString(string name)
        {
            return _cache.ContainsKey(name) ? _cache[name] : StringsManagers.NOT_AVAILABLE_STRING;
        }
        #endregion

        #region Private Methods
        
        #endregion


        #region Singleton
        private static volatile StringsManagers _settingNamesInstance;
        private static volatile StringsManagers _settingDefsInstance;
        private static volatile StringsManagers _locStringsInstance;
        private static object syncRoot = new Object();

        private StringsManagers() 
        {
            var _currCulture = Thread.CurrentThread.CurrentCulture;
        }

        public static StringsManagers SettingNames
        {
            get 
            {
                if (_settingNamesInstance == null) 
                {
                    lock (syncRoot) 
                    {
                        if (_settingNamesInstance == null)
                        {
                            _settingNamesInstance = new StringsManagers();
                            //_settingsInstance._resManager = new ResourceManager("Blogger.Resources.Settings.Names", Assembly.GetExecutingAssembly());                        
                        }
                    }
                }

                return _settingNamesInstance;
            }
        }

        public static StringsManagers SettingDefaults
        {
            get 
            {
                if (_settingDefsInstance == null) 
                {
                    lock (syncRoot) 
                    {
                        if (_settingDefsInstance == null)
                        {
                            _settingDefsInstance = new StringsManagers();                            
                        }
                    }
                }

                return _settingDefsInstance;
            }
        }


        public static StringsManagers LocalizedStrings
        {
            get
            {
                if (_locStringsInstance == null)
                {
                    lock (syncRoot)
                    {
                        if (_locStringsInstance == null)
                        {
                            _locStringsInstance = new StringsManagers();
                            //_locStringsInstance._resManager = new ResourceManager("Blogger.Resources.Localization.Strings", typeof(StringsManager).Assembly);                            
                        }
                    }
                }

                return _locStringsInstance;
            }
        }
        #endregion
    }
}
