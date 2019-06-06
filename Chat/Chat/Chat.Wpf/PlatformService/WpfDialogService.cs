using Chat.PlatformService;
using Chat.Wpf.PlatformService;
using System.Windows;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WPF;

[assembly: Dependency(typeof(WpfDialogService))]
namespace Chat.Wpf.PlatformService
{
    public class WpfDialogService : UserDialogService
    {
        public override void Toast(string message)
        {
            MessageBox.Show(message);
        }

        public override void Message(string message, string title = "Error")
        {
            MessageBox.Show(message);
        }
    }
}
