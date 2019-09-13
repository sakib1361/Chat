using Newtonsoft.Json;

namespace ChatCore.Engine
{
    public class User
    {
        public User()
        {

        }
        public User(string userName, string firstName, string lastName)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
        }

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public string Image { get; set; }
        public string FullName => string.Format("{0} {1}", FirstName, LastName);
    }
}
