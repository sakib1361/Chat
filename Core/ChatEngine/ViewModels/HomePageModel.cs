using ChatClient.Engine;
using ChatEngine.Services;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ChatEngine.ViewModels
{
    public class HomePageModel : BaseViewModel
    {
        private readonly ChatService chatService;
        public User CurrentUser { get; private set; }
        public ObservableCollection<User> Users { get; private set; }

        public HomePageModel(ChatService chatService)
        {

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

        public async override void RefreshPage()
        {
            IsBusy = true;
            base.RefreshPage();
            Users.Clear();
            IsRefreshing = false;
            var getUser = new ChatObject()
            {
                MessageType = MessageType.GetUsers,
                SenderName = AppService.CurrentUser
            };
            var res = await chatService.GetData(getUser);
            if(res != null)
            {
                if (res.MessageType == MessageType.Failed) HandlerErrors(res);
                else Users_Received(res);
            }
            IsBusy  = false;
        }

        public ICommand SelectedCommand => new RelayCommand<User>(SelectedAction);

        private void SelectedAction(User user)
        {
            NavigateToPage(typeof(ChatPageModel), user.Username);
        }
    }
}
