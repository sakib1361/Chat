using ChatEngine.Services;
using System;
using Xamarin.Forms;

namespace Chat.PlatformService
{
    public class DispatcherXamarin : IDispatcher
    {
        public void RunAsync(Action action)
        {
            Device.BeginInvokeOnMainThread(action);
        }
    }
}
