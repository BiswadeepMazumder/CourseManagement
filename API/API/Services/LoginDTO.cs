using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }

        // Constructor to initialize the properties
        public LoginDTO(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}