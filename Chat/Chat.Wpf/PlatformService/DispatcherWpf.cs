using ChatClient.Services;

namespace Chat.Wpf.PlatformService
{
    public class DispatcherWpf : IDispatcher
    {
        public async void RunAsync(Action action)
        {
            await App.Current.Dispatcher.InvokeAsync(action);
        }
    }
}
