using Chat.Services;
using ChatClient.Engine;
using Xamarin.Forms;

namespace Chat.Pages.ChatPages
{
    class ChatTemplateSelector : DataTemplateSelector
    {
        readonly DataTemplate incomingDataTemplate;
        readonly DataTemplate outgoingDataTemplate;

        public ChatTemplateSelector()
        {
            this.incomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            this.outgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (!(item is ChatObject messageVm))
                return null;
            return (messageVm.SenderName == AppService.CurrentUser) ? outgoingDataTemplate : incomingDataTemplate;
        }
    }
}