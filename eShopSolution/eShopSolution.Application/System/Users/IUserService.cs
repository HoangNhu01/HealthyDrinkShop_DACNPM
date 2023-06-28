using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.AppSystem.ExternalUser;
using eShopSolution.ViewModels.AppSystem.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.AppSystem.Users
{
    public interface IUserService
    {
        Task<ApiResult<string>> Authenticate(LoginRequest request);
        Task<ApiResult<string>> ExternalAuthenticate(FaceBookUserInfor request);
        Task<ApiResult<string>> ExternalAuthenticate(GoogleUserInfor request);

        Task<ApiResult<string>> Register(RegisterRequest request);
        Task<ApiResult<string>> SendConfirmEmail(string email);
        Task<ApiResult<string>> VerìfyEmail(string email, string token);

        Task<ApiResult<bool>> Update(Guid id, UserUpdateRequest request);

        Task<ApiResult<PagedResult<UserVm>>> GetUsersPaging(GetUserPagingRequest request);

        Task<ApiResult<UserVm>> GetById(Guid id);

        Task<ApiResult<bool>> Delete(Guid id);

        Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request);
        Task<ApiResult<UserVm>> GetByName(string name);


    }
}