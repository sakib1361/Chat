using ChatEngine.ViewModels;
using System;
using System.Threading.Tasks;

namespace ChatEngine.Services
{
    public interface INaviagationPage
    {
        Task NavigateTo(Type type, object[] args);
        Task NavigateBindTo(BaseViewModel baseViewModel, Type type, object[] args);
        void RemovePage();
        void GoBack();
    }
}
