using Chat.Wpf.Pages.Login;
using Chat.Wpf.ViewModels;
using ChatEngine.Services;
using ChatEngine.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Chat.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ViewModelLocator.InitializeNavigation(typeof(LoginPageModel), typeof(LoginWindow));
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            var chatService = SimpleIoc.Default.GetInstance<ChatService>();
            chatService.Stop();
        }
    }
}
