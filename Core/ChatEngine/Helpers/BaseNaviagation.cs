using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient.Helpers
{
    public abstract class BaseNaviagation
    {
        private readonly Dictionary<string, Type> pageTypes;
        public BaseNaviagation()
        {
            pageTypes = new Dictionary<string, Type>();
        }
        public void Register(Type page, Type viewModel)
        {
            pageTypes.Add(viewModel.Name, page);
        }

        public Type GetPage(Type viewmodel)
        {
            return pageTypes[viewmodel.Name];
        }
    }
}
