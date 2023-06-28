using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.AppSystem.ExternalUser;
using eShopSolution.ViewModels.AppSystem.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public interface IUserApiClient
    {
        Task<ApiResult<string>> Authenticate(LoginRequest request);

        Task<ApiResult<PagedResult<UserVm>>> GetUsersPagings(GetUserPagingRequest request);

        Task<ApiResult<string>> RegisterUser(RegisterRequest registerRequest);
        Task<ApiResult<string>> ExternalAuthenticate(FaceBookUserInfor request);
        Task<ApiResult<string>> ExternalAuthenticate(GoogleUserInfor request);

        Task<ApiResult<bool>> UpdateUser(Guid id, UserUpdateRequest request);

        Task<ApiResult<UserVm>> GetById(Guid id);
        Task<ApiResult<UserVm>> GetByName(string userName);
        Task<ApiResult<bool>> Delete(Guid id);

        Task<ApiResult<string>> VerìfyEmail(string email, string token);
        Task<ApiResult<bool>> RoleAssign(Guid id, RoleAssignRequest request);
        Task<ValidateTokenResult> GetAccessFBTokenAsync(string facebookAppId, string facebookAppSecret, string facebookAppDomain, string code);
        Task<ValidateTokenResult> GetAccessGGTokenAsync(string googleClientId, string googleAppSecret, string googleAppDomai, string code);

        Task<FaceBookUserInfor> GetFbUserAsync(string accessToken);
        Task<GoogleUserInfor> GetGgUserAsync(string accessToken);
    }
}