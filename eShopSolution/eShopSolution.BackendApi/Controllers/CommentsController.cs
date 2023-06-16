using eShopSolution.Application.Catalog.Categories;
using eShopSolution.Application.Community.Comments;
using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Community.Comments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Threading.Tasks;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentsController(
            ICommentService commentService)
        {
            _commentService = commentService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(Guid userId, int productId, string languageId)
        {
            var comments = await _commentService.GetByAnyId(userId, productId, languageId);
            return Ok(comments);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommetCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var apiResult = await _commentService.Create(request);
            if (apiResult.ResultObj == Guid.Empty)
                return BadRequest();

            return Ok(apiResult);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CommentUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedResult = await _commentService.Update(request);
            if (affectedResult.ResultObj == 0)
                return BadRequest();
            return Ok(affectedResult);
        }
    }
}
