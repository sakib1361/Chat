using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Chat.Controls
{
    public class GradientPage : ContentPage
    {
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }
        public bool Horizontal { get; set; }
    }
}
