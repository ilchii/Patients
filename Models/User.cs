using System.ComponentModel.DataAnnotations;

namespace Patients.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }  // Required for EF

        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
