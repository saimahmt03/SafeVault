using Microsoft.AspNetCore.Mvc;
using SafeVaultAPI.Entities;
using SafeVaultAPI.DTO.Request;
using SafeVaultAPI.DTO.Result;
using SafeVaultAPI.Service;
using SafeVaultAPI.Shared;
using Microsoft.AspNetCore.Authorization;

namespace SafeVaultAPI.Controller
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("safevault")]
    public class UserController : ControllerBase
    {
        private readonly IService _service;

        public UserController(IService service)
        {
            _service = service;
        }

        [HttpPost("adduser")]
        [AllowAnonymous] // to user
        public async Task<IActionResult> AddUser([FromBody] AddNewUserRequest request)
        {
            Result result = new Result();

            result = await _service.AddNewUserAsync(request);

            if (result.Code == BaseResult.ResultCode.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }


        [Authorize(Policy = "UserPolicy")]
        [HttpGet("getallusers")]
        public async Task<IActionResult> GetAllUser()
        {
            var result = await _service.GetAllUserAsync();
            return Ok(result);
        }
    }
}