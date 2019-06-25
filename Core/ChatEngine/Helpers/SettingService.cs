using System.Runtime.CompilerServices;

namespace ChatClient.Helpers
{
    public class SettingService
    {
        public static SettingService Instance = new SettingService();
        private ISettingsEngine settingsEngine;
        #region Server
        public string ServerName
        {
            get => GetSettings(CallName(), "192.168.1.1");
            set => SetSetting(CallName(), value);
        }

        public int Port
        {
            get => GetIntSetting(CallName(), 1200);
            set => SetSetting(CallName(), value.ToString());
        }
        #endregion

        #region Engine
        public void Init(ISettingsEngine settingsEngine)
        {
            this.settingsEngine = settingsEngine;
        }
        private string CallName([CallerMemberName]string name = "")
        {
            return name;
        }

        private int GetIntSetting(string key, int defaultVal)
        {
            var s = GetSettings(key, "");
            if (!string.IsNullOrWhiteSpace(s) && int.TryParse(s, out int res))
                return res;
            else
                return defaultVal;
        }
        private string GetSettings(string key, string defaultVal)
        {
            return settingsEngine.GetSettings(key, defaultVal);
        }

        private void SetSetting(string key, string value)
        {
            settingsEngine.SetSetting(key, value);
        }
        #endregion
    }

    public interface ISettingsEngine
    {
        string GetSettings(string key, string defaultVal);
        void SetSetting(string key, string value);
    }
}
