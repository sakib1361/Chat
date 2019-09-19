using ChatClient.Helpers;
using ChatClient.Services;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    public class ServerPageModel : BaseViewModel
    {
        private readonly ChatService ChatService;

        public string Servername { get; set; }
        public int Port { get; set; }
        public bool AllowSSL { get; set; } = true;
        public bool AllowPort { get; set; } = false;
        public ServerPageModel(ChatService chatService)
        {
            ChatService = chatService;
        }
        public override void OnAppearing(params object[] parameter)
        {
            base.OnAppearing(parameter);
            Servername = SettingService.Instance.ServerName;
            Port = SettingService.Instance.Port;
            AllowSSL = SettingService.Instance.AllowSSL;
            AllowPort = SettingService.Instance.AllowPort;
        }

        public ICommand SaveCommand => new RelayCommand(SaveAction);

        private void SaveAction()
        {
            if (string.IsNullOrWhiteSpace(Servername))
                ShowToastMessage(Translate("Invalid_Servername"));
            else if (Port < 70)
                ShowToastMessage(Translate("Invalid_Port"));
            else
            {
                SettingService.Instance.Port = Port;
                SettingService.Instance.ServerName = Servername;
                SettingService.Instance.AllowSSL = AllowSSL;
                SettingService.Instance.AllowPort = AllowPort;
                MoveToPage(typeof(LoginPageModel));
            }
        }
    }
}
