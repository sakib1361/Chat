using ChatEngine.Helpers;
using ChatEngine.Services;
using ChatEngine.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chat.Wpf.Services
{
    public class NavigationHelper : BaseNaviagation, INaviagationPage
    {
        private Window MainWindow;
        private readonly IList<Type> WindowTypes = new List<Type>();
        internal NavigationHelper(Window window, Type baseView)
        {
            this.MainWindow = window;
            MainWindow.Show();
            WindowTypes.Add(baseView);
        }

        public async void GoBack()
        {
            var lastWindow = WindowTypes.LastOrDefault();
            if (lastWindow != null)
            {
                WindowTypes.Remove(lastWindow);
                lastWindow = WindowTypes.LastOrDefault();
                if (lastWindow != null)
                    await NavigateTo(lastWindow);
            }
        }

        public Task NavigateBindTo(BaseViewModel baseViewModel, Type type, object[] args)
        {
            throw new NotImplementedException();
        }

        public async Task NavigateTo(Type basemodelType,params object[] args)
        {
            var pageType = GetPage(basemodelType);
            var datacontext = SimpleIoc.Default.GetInstance(basemodelType);
            if (datacontext is BaseViewModel baseView)
                baseView.OnAppearing(args);
            
            if(pageType.BaseType == typeof(MetroWindow))
            {
                var mWindow = Activator.CreateInstance(pageType) as Window;
                mWindow.DataContext = datacontext;
                mWindow.Show();
                await Task.Delay(100);
                MainWindow.Close();
                MainWindow = mWindow;
                WindowTypes.Add(basemodelType);
            }
        }

        public void RemovePage()
        {
            WindowTypes.Clear();
        }
    }
}