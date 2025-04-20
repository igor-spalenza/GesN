using Microsoft.AspNetCore.Identity;

namespace GesN.Web.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken);
        Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken);
        Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken);
    }
}
