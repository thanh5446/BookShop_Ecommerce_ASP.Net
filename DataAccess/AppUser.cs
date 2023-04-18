using Assignment.Models;
using Microsoft.AspNetCore.Identity;
namespace Assignment.DataAccess
{
    public class AppUser: IdentityUser
    {
        public string FullName { get; set; }
        public string? ProfilePicture { get; set; }
        public string Location { get; set; }

        public virtual List<Order>? Order { get; set; } = new List<Order>();
    }
}
