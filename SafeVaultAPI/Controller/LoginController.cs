using Microsoft.AspNetCore.Mvc;
using SafeVaultAPI.DTO.Request;
using SafeVaultAPI.DTO.Result;
using SafeVaultAPI.Service;
using SafeVaultAPI.Shared;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;

namespace SafeVaultAPI.Controller
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("safevault")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IMemoryCache _memoryCache;
        private readonly string _cacheKey1 = "UsernameCache";

        public LoginController(ILoginService loginService, IMemoryCache memoryCache, IJwtTokenGenerator jwtTokenGenerator)
        {
            _loginService = loginService;
            _memoryCache = memoryCache;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            LoginResult result = new LoginResult(); 
            TokenResult tokenResult = new TokenResult();

            result = await _loginService.LoginAsync(request);
            if(!result.Status)
            {
                return Unauthorized(BaseResult.ResultMessage.Unauthorized);
            }
            else
            {
                _memoryCache.Set(_cacheKey1, request.username);

                if(result.Role == Role.RoleValue.Value1)
                {
                    tokenResult = _jwtTokenGenerator.GenerateToken(result.Username, Role.RoleValue.Value1, result.ApplicationName);
                }
                else if(result.Role == Role.RoleValue.Value2)
                {
                    tokenResult = _jwtTokenGenerator.GenerateToken(result.Username, Role.RoleValue.Value2, result.ApplicationName);
                }
                else
                {
                    tokenResult = _jwtTokenGenerator.GenerateToken(result.Username, Role.RoleValue.Value3, result.ApplicationName); 
                }
                
                return Ok(tokenResult);
            }     
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("tokenvalidate")]
        public async Task<IActionResult> ValidateToken()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required.");
            }

            bool isValid = _jwtTokenGenerator.TokenValidator(token);
            
            if (isValid)
            {
                return Ok("Valid Token!");
            }
            else
            {
                return Unauthorized("Invalid Token!");
            }
        }
    }
}