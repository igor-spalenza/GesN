using Microsoft.AspNetCore.Identity;

namespace GesN.Web.Interfaces.Services
{
    public interface IUserService
    {
        Task<IdentityUser> GetUserByIdAsync(string userId, CancellationToken cancellationToken);
        Task<IdentityResult> CreateUserAsync(IdentityUser user, string password, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateUserAsync(IdentityUser user, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteUserAsync(string userId, CancellationToken cancellationToken);
    }
}
