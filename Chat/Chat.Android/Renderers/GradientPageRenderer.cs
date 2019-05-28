using System;
using Android.Content;
using Chat.Controls;
using Chat.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GradientPage), typeof(GradientPageRenderer))]
namespace Chat.Droid.Renderers
{
    public class GradientPageRenderer : PageRenderer
    {
        private Color StartColor { get; set; }
        private Color EndColor { get; set; }
        private bool Horizontal { get; set; }

        public GradientPageRenderer(Context context) : base(context)
        {

        }

        protected override void DispatchDraw(global::Android.Graphics.Canvas canvas)
        {
            float width = Width, height = 0;
            if (Horizontal == false)
            {
                width = 0;
                height = Height;
            }

            var gradient = new Android.Graphics.LinearGradient(0, 0, width, height,
                this.StartColor.ToAndroid(),
                this.EndColor.ToAndroid(),
                Android.Graphics.Shader.TileMode.Mirror);
            var paint = new Android.Graphics.Paint()
            {
                Dither = true,
            };
            paint.SetShader(gradient);
            canvas.DrawPaint(paint);
            base.DispatchDraw(canvas);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                var page = e.NewElement as GradientPage;
                this.StartColor = page.StartColor;
                this.EndColor = page.EndColor;
                this.Horizontal = page.Horizontal;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"       ERROR: ", ex.Message);
            }
        }
    }
}