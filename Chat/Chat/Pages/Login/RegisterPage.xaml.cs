using Chat.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chat.Pages.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : GradientPage
    {
        private readonly StarGradient StarGradient;

        public RegisterPage()
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