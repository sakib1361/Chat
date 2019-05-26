using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient.Engine
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }//Plain pass for register
        [JsonIgnore]
        public string StoredPassword { get; set; } //Hashed {assword

        public bool Active { get; set; }
    }
}
