using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public interface ICategoryApiClient
    {
        Task<ApiResult<List<CategoryVm>>> GetAll(string languageId);

        Task<ApiResult<CategoryVm>> GetById(string languageId, int id);

        Task<ApiResult<bool>> CreateCategory(CategoryCreateRequest request);

        Task<int> UpdateCategory(CategoryUpdateRequest request);

        Task<ApiResult<bool>> DeleteCategory(int id);
        
    }
}
