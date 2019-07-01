using ChatClient.Helpers;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Chat.Wpf.Services
{
    class SettingsEngine : ISettingsEngine
    {
        string ISettingsEngine.GetSettings(string key, string defaultVal)
        {
            ApplicationSettingsBase settings = Properties.Settings.Default;
            if (settings.Properties.Cast<SettingsProperty>().All(s => s.Name != key))
                return AddNew(key, defaultVal);
            else
                return settings[key].ToString();
        }

        private string AddNew(string key, string value)
        {
            ApplicationSettingsBase settings = Properties.Settings.Default;
            var p = new SettingsProperty(key)
            {
                PropertyType = typeof(string),
                Provider = settings.Providers["LocalFileSettingsProvider"],
                SerializeAs = SettingsSerializeAs.Xml
            };

            p.Attributes.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());

            settings.Properties.Add(p);
            settings.Reload();

            //finally set value with new value if none was loaded from userConfig.xml
            var item = settings[key];
            if (item == null)
            {
                settings[key] = value;
                settings.Save();
                return value;
            }
            else return item.ToString();
        }

        void ISettingsEngine.SetSetting(string key, string value)
        {
            ApplicationSettingsBase settings = Properties.Settings.Default;
            if (settings.Properties.Cast<SettingsProperty>().All(s => s.Name != key))
                AddNew(key, value);
            else
            {
                settings[key] = value;
                settings.Save();
            }
        }
    }
}
