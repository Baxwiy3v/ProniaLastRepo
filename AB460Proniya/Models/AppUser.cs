using Microsoft.AspNetCore.Identity;

namespace AB460Proniya.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }

        public string Surname { get; set; }
    }
}
