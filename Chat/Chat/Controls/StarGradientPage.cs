using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Chat.Controls
{
    public class StarGradient
    {
        private readonly DisplayInfo _metrics;
        private readonly int _formsWidth;
        private readonly int _formsHeight;
        private List<VisualElement> _stars = new List<VisualElement>();
        internal IList<Grid> StarFields;

        public StarGradient()
        {
            _metrics = DeviceDisplay.MainDisplayInfo;
            StarFields = new List<Grid>();
            _formsWidth = Convert.ToInt32(_metrics.Width / _metrics.Density);
            _formsHeight = Convert.ToInt32(_metrics.Height / _metrics.Density);
            PositionStars();
        }

        private void PositionStars()
        {

            var random = new Random();


            for (int j = 0; j < 5; j++)
            {
                var starField = new Grid();

                for (int i = 0; i < 20; i++)
                {
                    var size = random.Next(3, 7);
                    var star = new Button()
                    {
                        BackgroundColor = Color.White,
                        Opacity = 0.3,
                        HeightRequest = size,
                        WidthRequest = size,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.Start,
                        TranslationX = random.Next(0, _formsWidth),
                        TranslationY = random.Next(0, _formsHeight)
                    };
                    //var stars = new CachedImage()
                    //{
                    //    Source = "star.png",
                    //    Opacity = 0.3,
                    //    HeightRequest = size,
                    //    WidthRequest = size,

                    //};
                    starField.Children.Add(star);
                }

                _stars.Add(starField);
                StarFields.Insert(0, starField);
            }

        }

        CancellationTokenSource Cts;
        public async void RotateStars(ContentPage page)
        {
            CancelPrevious();
            Cts = new CancellationTokenSource();
            var rotateTasks = new List<Task>();
            var random = new Random();
          

            foreach (var star in _stars)
            {
                var rate = random.Next(240000, 300000);
                rotateTasks.Add(RotateElement(star, (uint)rate,Cts.Token));
            }

            await Task.WhenAll(rotateTasks);
        }

        internal async Task RotateElement(VisualElement element, uint duration, CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                await element.RotateTo(360, duration, Easing.Linear);
                await element.RotateTo(0, 0); // reset to initial position
            }
        }

        public void CancelPrevious()
        {
            Cts?.Cancel();
        }
    }
}
