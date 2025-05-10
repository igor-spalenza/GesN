using Microsoft.AspNetCore.Identity;

namespace GesN.Web.Areas.Identity.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
    }
}
