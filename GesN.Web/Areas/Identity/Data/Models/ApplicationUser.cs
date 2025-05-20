using Microsoft.AspNetCore.Identity;

namespace GesN.Web.Areas.Identity.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Removendo propriedades duplicadas que já existem em IdentityUser
        // Estas propriedades podem causar problemas porque ocultam as da classe base
        // SecurityStamp, ConcurrencyStamp, LockoutEnd, LockoutEnabled e AccessFailedCount já existem na classe base
        public new string LockoutEnd { get; set; }
    }
}
