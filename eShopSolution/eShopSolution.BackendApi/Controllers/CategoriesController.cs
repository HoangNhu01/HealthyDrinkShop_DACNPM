using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopSolution.Application.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(
            ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string languageId)
        {
            var products = await _categoryService.GetAll(languageId);
            return Ok(products);
        }

        [HttpGet("{languageId}/{id}")]
        public async Task<IActionResult> GetById(string languageId, int id)
        {
            var category = await _categoryService.GetById(languageId, id);
            return Ok(category);
        }
        [HttpPost("created-categories")]
        public async Task<IActionResult> Create([FromBody] CategoryCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoryId = await _categoryService.Create(request);
            if (categoryId == 0)
                return BadRequest();

            var category = await _categoryService.GetById(request.LanguageId, categoryId);

            return CreatedAtAction(nameof(GetById), new { id = categoryId }, category);
        }

        [HttpPut("updated-categories")]
        public async Task<IActionResult> Update([FromBody] CategoryUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedResult = await _categoryService.Update(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok(affectedResult);
        }

        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> Delete(int categoryId)
        {
            var affectedResult = await _categoryService.Delete(categoryId);
            if (affectedResult == 0)
                return BadRequest();
            return Ok(affectedResult);
        }
    }
}