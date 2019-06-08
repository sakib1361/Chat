using Chat.Wpf.PlatformService;
using ChatEngine.Services;
using System.Threading.Tasks;
using System.Windows;

namespace Chat.Wpf.PlatformService
{
    public class WpfDialogService : IUserDialogService
    {
        public async Task<bool> ConfirmAsync(string message, string title)
        {
            await Task.Delay(50);
            return true;
        }

        public void Message(string message, string title)
        {
            MessageBox.Show(message,title);
        }

        public void Toast(string message)
        {
            MessageBox.Show(message);
        }
    }
}
