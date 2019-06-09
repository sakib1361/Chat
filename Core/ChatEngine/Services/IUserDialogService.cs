using System.Threading.Tasks;

namespace ChatEngine.Services
{
    public interface IUserDialogService
    {
        void Toast(string message);
        void Message(string message, string title);
        Task<bool> ConfirmAsync(string message, string title);
    }
}
