using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public interface ICategoryApiClient
    {
        Task<ApiResult<List<CategoryVm>>> GetAll(string languageId);

        Task<ApiResult<CategoryVm>> GetById(string languageId, int id);
    }
}
