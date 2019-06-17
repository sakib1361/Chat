using ChatCore.Engine;
using ChatClient.Services;
using System.Windows;
using System.Windows.Controls;

namespace Chat.Wpf.Pages.ChatPages
{
    class ChatTemplateSelector : DataTemplateSelector
    {
        public DataTemplate IncomingDataTemplate { get; set; }
        public DataTemplate OutgoingDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is ChatObject messageVm))
                return null;
            return (messageVm.SenderName == AppService.CurrentUser) ? OutgoingDataTemplate : IncomingDataTemplate;
        }

        //protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        //{
        //    if (!(item is ChatObject messageVm))
        //        return null;
        //    return (messageVm.SenderName == AppService.CurrentUser) ? outgoingDataTemplate : incomingDataTemplate;
        //}
    }
}