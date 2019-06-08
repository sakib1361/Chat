using ChatEngine.Services;
using System.Windows.Data;

namespace Chat.Wpf.Services
{
    public class LocExtension : Binding
    {
        public LocExtension(string name) : base("[" + name + "]")
        {
            this.Mode = BindingMode.OneWay;
            this.Source = TranslationService.Instance;
        }
    }
}