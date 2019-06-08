using ChatEngine.Helpers;
using ChatEngine.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Linq;
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
            Messenger.Default.Register<string>(this, AppConstants.NewMessage, NewMessagge);
        }

        private void NewMessagge(string obj)
        {
            var last = ChatList.ItemsSource.Cast<object>().LastOrDefault();
            ChatList.ScrollTo(last, ScrollToPosition.MakeVisible, true);
        }

        private void Handle_Completed(object sender, EventArgs e)
        {
            if (this.BindingContext is ChatPageModel pageModel)
                pageModel.SendCommand.Execute(null);
        }
    }
}