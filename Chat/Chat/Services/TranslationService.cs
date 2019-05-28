using Chat.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chat.Services
{
    public class TranslationService : INotifyPropertyChanged
    {
        public static TranslationService Instance { get; } = new TranslationService();

        private readonly ResourceManager resManager = new ResourceManager(typeof(AppResources));
        private CultureInfo currentCulture = new CultureInfo("en");

        //public string this[string key]
        //{
        //    get {return this.resManager.GetString(key, this.currentCulture); }
        //}

        private const char KeySeparator = '_';

        public string this[string key]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(key)) return key;
                if (key.IndexOf(KeySeparator) == -1)
                {
                    var res = resManager?.GetString(key, currentCulture);
                    return !string.IsNullOrWhiteSpace(res) ? res : $"{key}";
                }
                else
                {
                    var keys = key.Split(KeySeparator);
                    var results = new List<string>();
                    foreach (var k in keys)
                    {
                        var res = resManager?.GetString(k, currentCulture);
                        results.Add(!string.IsNullOrWhiteSpace(res) ? res : $"{k}");
                    }
                    return string.Join(" ", results);
                }
            }
        }

        public CultureInfo CurrentCulture
        {
            get { return this.currentCulture; }
            set
            {
                if (this.currentCulture != value)
                {
                    this.currentCulture = value;
                    var @event = this.PropertyChanged;
                    if (@event != null)
                    {
                        @event.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

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