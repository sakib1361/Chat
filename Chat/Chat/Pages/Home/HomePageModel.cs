using Chat.Services;
using Chat.ViewModels;
using ChatClient.Engine;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Chat.Pages.Home
{
    public class HomePageModel : BaseViewModel
    {
        private readonly ChatService chatService;
        public User CurrentUser { get; private set; }
        public ObservableCollection<User> Users { get; private set; }

        public HomePageModel( ChatService chatService)
        {
            Messenger.Default.Register<ChatObject>(this, MessageType.GetUsers, Users_Received);
            this.chatService = chatService;
            Users = new ObservableCollection<User>();
        }

        public override void OnAppearing(params object[] parameter)
        {
            base.OnAppearing(parameter);
            RefreshPage();
        }

        private void Users_Received(ChatObject obj)
        {
            try
            {
                Users.Clear();
                var allUsers = JsonConvert.DeserializeObject<List<User>>(obj.Message);
                foreach (var item in allUsers)
                {
                    if (item.Username == AppService.CurrentUser)
                    {
                        CurrentUser = item;
                    }
                    else
                    {
                        Users.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        public override void RefreshPage()
        {
            base.RefreshPage();
            var getUser = new ChatObject()
            {
                MessageType = MessageType.GetUsers,
                SenderName = AppService.CurrentUser
            };
            chatService.SendMessage(getUser);
            IsBusy = false;
        }
    }
}
