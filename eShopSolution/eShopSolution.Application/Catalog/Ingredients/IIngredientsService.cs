using eShopSolution.ViewModels.Catalog.Ingredients;
using eShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Ingredients
{
    public interface IIngredientsService
    {
        Task<ApiResult<List<IngredientVm>>> GetAll();

        Task<ApiResult<IngredientVm>> GetById(int id);
    }
}