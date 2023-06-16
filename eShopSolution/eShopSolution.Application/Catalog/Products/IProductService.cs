using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IProductService
    {
        Task<int> Create(ProductCreateRequest request);

        Task<int> Update(ProductUpdateRequest request);

        Task<int> Delete(int productId);
        Task<int> DeleteImage(int imageId);

        Task<ApiResult<ProductVm>> GetById(int productId, string languageId);

        Task<bool> UpdatePrice(int productId, decimal newPrice);

        Task<bool> UpdateStock(int productId, int addedQuantity);

        Task<bool> UpdateFeature(int productId, bool isFeature);
        Task AddViewcount(int productId);

        Task<ApiResult<PagedResult<ProductVm>>> GetAllPaging(GetManageProductPagingRequest request);

        Task<int> AddImage(int productId, ProductImageCreateRequest request);


        Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request);

        Task<ProductImageVm> GetImageById(int imageId);

        Task<List<ProductImageVm>> GetListImages(int productId);

        Task<ApiResult<bool>> CategoryAssign(int id, CategoryAssignRequest request);

        Task<ApiResult<bool>> IngredientAssign(int id, IngredientAssignRequest request);

    }
}