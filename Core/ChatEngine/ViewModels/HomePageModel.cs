using ChatCore.Engine;
using ChatClient.Services;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ChatClient.ViewModels
{
    public class HomePageModel : BaseViewModel
    {
        public User CurrentUser { get; private set; }

        private readonly APIService _apiService;

        public ObservableCollection<User> Users { get; private set; }

        public HomePageModel(APIService aPIService)
        {

            _apiService = aPIService;
            Users = new ObservableCollection<User>();
        }

        public override void OnAppearing(params object[] parameter)
        {
            base.OnAppearing(parameter);
            RefreshPage();
        }

        public async override void RefreshPage()
        {
            IsBusy = true;
            base.RefreshPage();
            Users.Clear();
            IsRefreshing = false;

            var res = await _apiService.GetUsers();
            if(res == null)
            {
                ShowMessage(WorkerService.Instance.ErrorMessage);
            }
            else
            {
                foreach (var item in res)
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
            IsBusy  = false;
        }

        public ICommand SelectedCommand => new RelayCommand<User>(SelectedAction);

        private void SelectedAction(User user)
        {
            NavigateToPage(typeof(ChatPageModel), user.Username);
        }
    }
}
