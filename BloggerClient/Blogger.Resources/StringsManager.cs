using Blogger.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;

namespace Blogger.Resources
{
    public sealed class StringsManager
    {
        #region Constants
        public static readonly string NOT_AVAILABLE_STRING = "N/A";
        #endregion

        #region Fields
        ResourceManager _resManager;
        CultureInfo _currCulture;
        Dictionary<string, string> _cache = new Dictionary<string, string>();
        #endregion


        #region Static Methods
        public static string GetSettingName(Setting settingNameEnum)
        {
            return Settings.GetString(settingNameEnum.ToString());
        }
        #endregion


        #region Public Methods
        public string GetString(string name)
        {
            if (_cache.ContainsKey(name))
            {
                return _cache[name];
            }

            var resourceString = _resManager.GetString(name);
            if (string.IsNullOrEmpty(resourceString.Trim()))
            {
                resourceString = NOT_AVAILABLE_STRING;
            }

            _cache.Add(name, resourceString);

            return resourceString;
        }
        #endregion

        #region Private Methods
        
        #endregion


        #region Singleton
        private static volatile StringsManager _settingsInstance;
        private static volatile StringsManager _locStringsInstance;
        private static object syncRoot = new Object();

        private StringsManager() 
        {
            var _currCulture = Thread.CurrentThread.CurrentCulture;
        }

        public static StringsManager Settings
        {
            get 
            {
                if (_settingsInstance == null) 
                {
                lock (syncRoot) 
                {
                    if (_settingsInstance == null)
                    {
                        _settingsInstance = new StringsManager();
                        _settingsInstance._resManager = new ResourceManager("Settings.Names", typeof(StringsManager).Assembly);                        
                    }
                }
                }

                return _settingsInstance;
            }
        }


        public static StringsManager LocalizedStrings
        {
            get
            {
                if (_locStringsInstance == null)
                {
                    lock (syncRoot)
                    {
                        if (_locStringsInstance == null)
                        {
                            _locStringsInstance = new StringsManager();
                            _locStringsInstance._resManager = new ResourceManager("Localization.Strings", typeof(StringsManager).Assembly);                            
                        }
                    }
                }

                return _locStringsInstance;
            }
        }
        #endregion
    }
}
