using ChatEngine.Helpers;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Chat.Wpf.Services
{
    class SettingsEngine : ISettingsEngine
    {
        string ISettingsEngine.GetSettings(string key, string defaultVal)
        {
            if (Properties.Settings.Default.Properties[key] == null)
            {
                return defaultVal;
            }
            else
            {
                return Properties.Settings.Default.Properties[key].DefaultValue.ToString();
            }
        }

        void ISettingsEngine.SetSetting(string key, string value)
        {
            if (Properties.Settings.Default.Properties[key] == null)
            {
                var prop = new SettingsProperty(key)
                {
                    PropertyType = typeof(string)
                };
                Properties.Settings.Default.Properties.Add(prop);
                Properties.Settings.Default.Save();
            }

            Properties.Settings.Default.Properties[key].DefaultValue = value;
            Properties.Settings.Default.Save();
        }
    }
}
