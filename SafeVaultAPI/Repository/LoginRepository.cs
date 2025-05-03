using SafeVaultAPI.DTO.Request;
using SafeVaultAPI.DTO.Result;
using SafeVaultAPI.Entities;
using SafeVaultAPI.Shared;
using Microsoft.AspNetCore.Identity;

namespace SafeVaultAPI.Repository
{
    public class LoginRepository : ILoginRepository
    {

        private readonly IRepository _repository;
        public LoginRepository(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            LoginResult result = new LoginResult();

            var userList = await _repository.GetAllUserAsync();
            var hasher = new PasswordHasher<User>();

            foreach (var user in userList.Users)
            {
                var verification = hasher.VerifyHashedPassword(user, user.HashedPassword, request.password);
                if (verification == PasswordVerificationResult.Success)
                {
                    result.Username = user.username;
                    result.ApplicationName = user.applicationSignature;
                    result.Status = true;

                    if (user.type == Role.RoleValue.Value1 ||
                        user.type == Role.RoleValue.Value2 ||
                        user.type == Role.RoleValue.Value3)
                    {
                        result.Role = user.type;
                    }
                    else
                    {
                        result.Role = 0;
                        result.ApplicationName = string.Empty;
                        result.Status = false;
                    }

                    return result; // stop loop after successful match
                }
            }

            // If no match found, return failed login result
            result.Username = string.Empty;
            result.Role = 0;
            result.ApplicationName = string.Empty;
            result.Status = false;

            return result;
        }
    }
}