using ChatCore.Engine;
using ChatClient.ViewModels;

namespace Chat.Wpf.Pages.ChatPages
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private void UsersView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (UsersView.SelectedItem is User user && this.DataContext is HomePageModel pageModel)
                pageModel.SelectedCommand.Execute(user);
        }
    }
}
