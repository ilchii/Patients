using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patients.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }   
    }

    public static class UserStore
    {
        public static List<User> Users = new List<User>
        {
            new User { Username = "reception1", Password = "pass123", Role = "Receptionist" },
            new User { Username = "doctor1", Password = "docpass", Role = "Doctor" }
        };
    }
}
