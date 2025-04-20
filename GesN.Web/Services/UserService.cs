using GesN.Web.Interfaces.Repositories;
using GesN.Web.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace GesN.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IdentityUser> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _userRepository.FindByIdAsync(userId, cancellationToken);
        }

        public async Task<IdentityResult> CreateUserAsync(IdentityUser user, string password, CancellationToken cancellationToken)
        {
            // Aqui você pode adicionar lógica adicional, como hashing de senha
            return await _userRepository.CreateAsync(user, cancellationToken);
        }

        public async Task<IdentityResult> UpdateUserAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return await _userRepository.UpdateAsync(user, cancellationToken);
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByIdAsync(userId, cancellationToken);
            return await _userRepository.DeleteAsync(user, cancellationToken);
        }
    }
}
