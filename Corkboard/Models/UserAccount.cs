using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Corkboard.Models
{
    public class UserAccount : IdentityUser
    {
        public string? FullName { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public string? Bio { get; set; }

        public string? Major { get; set; }
    }
}
