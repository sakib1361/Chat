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
        public ServerPageModel(ChatService chatService)
        {
            ChatService = chatService;
        }
        public override void OnAppearing(params object[] parameter)
        {
            base.OnAppearing(parameter);
            Servername = SettingService.Instance.ServerName;
            Port = SettingService.Instance.Port;
        }

        public ICommand SaveCommand => new RelayCommand(SaveAction);

        private void SaveAction()
        {
            if (string.IsNullOrWhiteSpace(Servername))
                ShowToastMessage(Translate("Invalid_Servername"));
            else if (Port < 22)
                ShowToastMessage(Translate("Invalid_Port"));
            else
            {
                SettingService.Instance.Port = Port;
                SettingService.Instance.ServerName = Servername;
                GoBack();
            }
        }
    }
}
