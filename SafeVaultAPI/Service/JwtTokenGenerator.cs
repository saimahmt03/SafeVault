using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SafeVaultAPI.Utilities;
using SafeVaultAPI.DTO.Result;
using SafeVaultAPI.Extension;




namespace SafeVaultAPI.Service
{
    internal class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtTokenGenerator> _logger;

        private readonly List<ClientSettings> _clientSettings;

        public JwtTokenGenerator(IConfiguration configuration, ILogger<JwtTokenGenerator> logger, List<ClientSettings> clientSettings)
        {
            _configuration = configuration;
            _jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>() 
                            ?? throw new InvalidOperationException("JwtSettings configuration is not setup or invalid.");
            _logger = logger;
            _clientSettings = clientSettings;
        }

        public TokenResult GenerateToken(string username, int role, string appsignatory)
        {
            try
            {
                var settings = _jwtSettings.Clients.FirstOrDefault(j => j.Client == appsignatory);
                
                if (settings == null)
                {
                    throw new Exception($"Client '{appsignatory}' not found.");
                }

                var keyBytes = GetSecureKey(settings.Key);
                var key = new SymmetricSecurityKey(keyBytes);
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var issuedAt = DateTime.UtcNow;
                var expiresAt = issuedAt.AddMinutes(settings.DurationInMinutes);

                string userRole = DataSeeder.GetRoleByValue(role);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(ClaimTypes.Role, userRole),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)issuedAt).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                };

                var token = new JwtSecurityToken(
                    issuer: settings.Issuer,
                    audience: settings.Audience,
                    claims: claims,
                    notBefore: issuedAt,
                    expires: expiresAt,
                    signingCredentials: credentials
                );

                string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                
                // Call method to set token in an HttpOnly cookie
                //SetTokenInCookie(tokenString, expiresAt);

                TokenResult tokenResult = new TokenResult
                {
                    Token = tokenString,
                    Role = settings.Client,
                    Duration = (int)(expiresAt - issuedAt).TotalMinutes, // Cuurent time minus the Expiration declared in appsetings
                    IssueBy = settings.Issuer
                };

                return tokenResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token generation failed for client: {Client}", appsignatory);
                throw;
            }
        }
        
        private byte[] GetSecureKey(string keyNameOrValue)
        {
            // Try from environment variable first
            var envKey = Environment.GetEnvironmentVariable(keyNameOrValue);
            if (!string.IsNullOrWhiteSpace(envKey))
            {
                return Encoding.UTF8.GetBytes(envKey);
            }

            // Fallback to raw key string
            return Encoding.UTF8.GetBytes(keyNameOrValue);
        }

        public void SetTokenInCookie(HttpResponse response, string tokenString, DateTime expiresAt)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Ensures the cookie is not accessible via JavaScript
                Secure = true,   // Ensure cookie is only sent over HTTPS
                SameSite = SameSiteMode.Strict, // Prevents CSRF attacks
                Expires = expiresAt // Set expiration to match the token expiry
            };
            response.Cookies.Append("jwt", tokenString, cookieOptions);
        }

        public bool TokenValidator(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuers = _clientSettings.Select(c => c.Issuer),
                    ValidAudiences = _clientSettings.Select(c => c.Audience),
                    IssuerSigningKeys = _clientSettings.Select(c =>
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(c.Key))),

                        // Set ClockSkew to zero to strictly validate the expiration time
                    ClockSkew = TimeSpan.Zero
                };

                // Validate the token
                var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

                // If it reaches this point, the token is valid
                return validatedToken != null;
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return false; // Invalid token
            }
        }
    }
}