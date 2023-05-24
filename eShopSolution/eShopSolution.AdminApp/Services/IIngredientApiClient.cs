using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.Ingredients;
using eShopSolution.ViewModels.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public interface IIngredientApiClient
    {
        Task<ApiResult<List<IngredientVm>>> GetAll();

        Task<ApiResult<IngredientVm>> GetById(int id);
    }
}
