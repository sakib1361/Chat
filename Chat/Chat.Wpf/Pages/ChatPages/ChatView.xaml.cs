using ChatClient.Helpers;
using ChatClient.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows.Controls;

namespace Chat.Wpf.Pages.ChatPages
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : UserControl
    {
        public ChatView()
        {
            InitializeComponent();
            Messenger.Default.Register<string>(this, AppConstants.NewMessage, MsgAdded);
        }

        private void MsgAdded(string obj)
        {
            var item = ChatList.Items[ChatList.Items.Count - 1];
            ChatList.ScrollIntoView(item);
        }
    }
}
