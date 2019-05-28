using Chat.Controls;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace Chat.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : GradientPage
    {
        public StarGradient StarGradient { get; }
        public LoginPage()
        {
            InitializeComponent();
            StarGradient = new StarGradient();
            foreach (var item in StarGradient.StarFields)
                MainGrid.Children.Insert(0, item);
        }


        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(250);
            StarGradient.RotateStars(this);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StarGradient.CancelPrevious();
        }
    }
}