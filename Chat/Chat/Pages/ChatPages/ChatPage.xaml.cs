using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chat.Pages.ChatPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public ChatPage()
        {
            InitializeComponent();
        }

        private void Handle_Completed(object sender, EventArgs e)
        {
            if (this.BindingContext is ChatPageModel pageModel)
                pageModel.SendCommand.Execute(null);
        }
    }
}