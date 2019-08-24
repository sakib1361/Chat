using Microsoft.AspNetCore.Identity;

namespace ChatServer.Engine.Database
{
    public class IDUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Active { get; set; }
        public string Image { get; set; }
        public string FullName => string.Format("{0} {1}", FirstName, LastName);
    }
}
