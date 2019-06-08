using ChatClient.Engine;
using ChatEngine.Services;
using Xamarin.Forms;

namespace Chat.Pages.ChatPages
{
    class ChatTemplateSelector : DataTemplateSelector
    {
        public DataTemplate IncomingDataTemplate { get; set; }
        public DataTemplate OutgoingDataTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (!(item is ChatObject messageVm))
                return null;
            return (messageVm.SenderName == AppService.CurrentUser) ? OutgoingDataTemplate : IncomingDataTemplate;
        }
    }
}