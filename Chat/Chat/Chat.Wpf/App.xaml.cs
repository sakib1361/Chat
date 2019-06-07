using Chat.Wpf.ViewModels;
using ChatEngine.ViewModels;
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
            ViewModelLocator.InitializeNavigation(typeof(LoginPageModel), typeof(MainWindow));
            var m = new MainWindow();
            m.Show();
        }
    }
}
