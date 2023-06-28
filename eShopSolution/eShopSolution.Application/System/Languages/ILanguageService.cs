using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.AppSystem.Languages;
using eShopSolution.ViewModels.AppSystem.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.AppSystem.Languages
{
    public interface ILanguageService
    {
        Task<ApiResult<List<LanguageVm>>> GetAll();
    }
}