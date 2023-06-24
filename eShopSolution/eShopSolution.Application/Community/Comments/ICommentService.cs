using eShopSolution.Data.Enums;
using eShopSolution.ViewModels.Common;
using eShopSolution.ViewModels.Community.Comments;
using eShopSolution.ViewModels.Sales;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.Community.Comments
{
    public interface ICommentService
    {
        Task<ApiResult<List<CommentVm>>> GetByAnyId(Guid id, int productId, string languageId);
        Task<ApiResult<Guid>> Create(CommetCreateRequest request);

        Task<ApiResult<int>> Update(CommentUpdateRequest request);

        Task<ApiResult<bool>> Delete(Guid commentId);
    }
}
