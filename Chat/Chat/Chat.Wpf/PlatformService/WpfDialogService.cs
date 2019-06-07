using Chat.Wpf.PlatformService;
using ChatEngine.Services;
using System.Threading.Tasks;

namespace Chat.Wpf.PlatformService
{
    public class WpfDialogService : IUserDialogService
    {
        public Task<bool> ConfirmAsync(string message, string title)
        {
            throw new System.NotImplementedException();
        }

        public void Message(string message, string title)
        {
            throw new System.NotImplementedException();
        }

        public void Toast(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}
