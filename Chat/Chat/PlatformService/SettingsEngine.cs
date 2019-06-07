using ChatEngine.Helpers;
using Xamarin.Forms;

namespace Chat.PlatformService
{
    public class SettingsEngine : ISettingsEngine
    {
        public string GetSettings(string key, string defaultVal)
        {
            if (Application.Current.Properties.ContainsKey(key))
            {
                var value = Application.Current.Properties[key] as string;
                return value;
            }
            else return defaultVal;
        }

        public async void SetSetting(string key, string value)
        {
            Application.Current.Properties[key] = value;
            await Application.Current.SavePropertiesAsync();
        }
    }
}
