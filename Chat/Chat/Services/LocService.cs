using ChatEngine.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chat.Services
{
    [ContentProperty(nameof(Attribute))]
    public class LocExtension : IMarkupExtension<BindingBase>
    {
        public string Attribute { get; set; }
        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }

        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding
            {
                Mode = BindingMode.OneWay,
                Path = $"[{Attribute}]",
                Source = TranslationService.Instance,
            };
            return binding;
        }
    }
}
