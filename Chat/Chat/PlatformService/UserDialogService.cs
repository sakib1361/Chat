using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Chat.PlatformService
{
    public class UserDialogService
    {
        public static UserDialogService Instance = DependencyService.Get<UserDialogService>();

        public virtual void Toast(string message)
        {
            UserDialogs.Instance.Toast(message);
        }

        public virtual void Message(string message, string title = "Error")
        {
            UserDialogs.Instance.Alert(message);
        }

        internal Task<bool> ConfirmAsync(string message, string title)
        {
            throw new NotImplementedException();
        }
    }
}
