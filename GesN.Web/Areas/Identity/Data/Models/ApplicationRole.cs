using Microsoft.AspNetCore.Identity;

namespace GesN.Web.Areas.Identity.Data.Models
{
    public class ApplicationRole : IdentityRole
    {
        // ConcurrencyStamp já existe na classe base IdentityRole
        // Data/Hora de criação da role
        public DateTime? CreatedDate { get; set; }
    }
}
