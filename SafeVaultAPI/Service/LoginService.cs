using SafeVaultAPI.DTO.Request;
using SafeVaultAPI.DTO.Result;
using SafeVaultAPI.Repository;
using SafeVaultAPI.Utilities;

namespace SafeVaultAPI.Service
{
    internal class LoginService : ILoginService
    {
        private readonly ILoginRepository _repository;
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration;
        
        public LoginService(ILoginRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
            _jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>() 
                            ?? throw new InvalidOperationException("JwtSettings configuration is not setup or invalid.");
        }

        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            LoginResult result = new LoginResult();
            
            result = await _repository.LoginAsync(request);

            return result;
        }
    }
}