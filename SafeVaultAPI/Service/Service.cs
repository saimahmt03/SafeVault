using SafeVaultAPI.DTO.Request;
using SafeVaultAPI.DTO.Result;
using SafeVaultAPI.Entities;
using SafeVaultAPI.Shared;
using SafeVaultAPI.Repository; 
using Microsoft.Extensions.Caching.Memory;

namespace SafeVaultAPI.Service
{
    internal class Service : IService
    {
        private readonly IRepository _repository;
        
        public Service(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> AddNewUserAsync(AddNewUserRequest request)
        {
            Result result = new Result();

            result = await _repository.AddNewUserAsync(request);

            if (result.Code == BaseResult.ResultCode.Success)
            {
                result.Code = BaseResult.ResultCode.Success;
                result.Message = result.Message;
            }
            else
            {
                result.Code = BaseResult.ResultCode.Invalid;
                result.Message = BaseResult.ResultMessage.Invalid;
            }

            return result;
        }

        public async Task<UserList> GetAllUserAsync()
        {
            UserList userList = new UserList();

            userList = await _repository.GetAllUserAsync();
            
            return userList;
        }   
    }
}