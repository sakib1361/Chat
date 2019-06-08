using ChatEngine.Services;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Threading.Tasks;

namespace Chat.Wpf.PlatformService
{
    public class DispatcherWpf : IDispatcher
    {
        public async void RunAsync(Action action)
        {
            await DispatcherHelper.RunAsync(action);
        }
    }
}
