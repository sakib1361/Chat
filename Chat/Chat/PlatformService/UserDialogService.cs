using Acr.UserDialogs;
using ChatEngine.Services;
using System.Threading.Tasks;

namespace Chat.PlatformService
{
    public class UserDialogService : IUserDialogService
    {

        public void Toast(string message)
        {
            UserDialogs.Instance.Toast(message);
        }

        public void Message(string message, string title = "Error")
        {
            UserDialogs.Instance.Alert(message);
        }

        public async Task<bool> ConfirmAsync(string message, string title)
        {
            return await UserDialogs.Instance.ConfirmAsync(message, title);
        }
    }
}
