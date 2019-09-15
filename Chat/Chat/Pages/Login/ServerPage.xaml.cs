using Chat.Services;
using ChatClient.Helpers;
using ChatClient.Services;
using GalaSoft.MvvmLight.Ioc;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chat.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServerPage : ContentPage
    {
        public ServerPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ServerAddress.Text = SettingService.Instance.ServerName;
            ServerPort.Text = SettingService.Instance.Port.ToString();
            SSLCheckBox.IsChecked = SettingService.Instance.AllowSSL;
            PortCheckBox.IsChecked = SettingService.Instance.AllowPort;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var server = ServerAddress.Text;
            int.TryParse(ServerPort.Text, out int port);
            if (string.IsNullOrWhiteSpace(server))
                DisplayAlert("Error", "Invalid Server", "Cancel");
            else if (port < 1)
                DisplayAlert("Error", "Invalid Port", "Cancel");
            else
            {
                SettingService.Instance.ServerName = server;
                SettingService.Instance.Port = port;
                SettingService.Instance.AllowPort = PortCheckBox.IsChecked;
                SettingService.Instance.AllowSSL = SSLCheckBox.IsChecked;
                DisplayAlert("Success", "Settings Saved", "Ok");
                var service = SimpleIoc.Default.GetInstance<ChatService>();
                service.Stop();
                service.Start();
            }
        }
    }
}