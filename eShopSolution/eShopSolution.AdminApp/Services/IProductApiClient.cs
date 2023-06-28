using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.AppSystem.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Services
{
    public interface IProductApiClient
    {
        Task<ApiResult<PagedResult<ProductVm>>> GetPagings(GetManageProductPagingRequest request);
        Task<ApiResult<ProductVm>> CreateProduct(ProductCreateRequest request);
        Task<bool> UpdateProduct(ProductUpdateRequest request);

        Task<bool> AddImage(int id, ProductImageCreateRequest request);

        Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request);
        Task<ApiResult<bool>> IngredientAssign(int id, IngredientAssignRequest request);

        Task<ApiResult<ProductVm>> GetById(int id, string languageId);

        Task<ApiResult<bool>> DeleteProduct(int id);
        Task<ApiResult<bool>> DeleteImage(int imageId);

    }
}