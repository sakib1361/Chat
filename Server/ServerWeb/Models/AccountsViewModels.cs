using System.ComponentModel.DataAnnotations;

namespace ServerWeb.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Password { get; set; }
        [Required]
        public string UserName { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
