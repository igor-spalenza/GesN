using Microsoft.AspNetCore.Identity;

namespace GesN.Web.Areas.Identity.Data.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string ConcurrencyStamp { get; set; }
    }
}
