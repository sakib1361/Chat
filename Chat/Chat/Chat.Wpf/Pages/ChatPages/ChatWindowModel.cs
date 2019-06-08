using ChatEngine.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Wpf.Pages.ChatPages
{
    public class ChatWindowModel
    {
        public HomePageModel HomePageModel { get; }
        public ChatPageModel ChatPageModel { get; }
        public ChatWindowModel(HomePageModel homePageModel, ChatPageModel chatPageModel)
        {
            HomePageModel = homePageModel;
            ChatPageModel = chatPageModel;
        }
    }
}
