using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopSolution.Application.Catalog.Ingredients;
using eShopSolution.Data.Entities;
using eShopSolution.ViewModels.Catalog.Ingredients;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientsService _ingredientsService;

        public IngredientsController(
            IIngredientsService categoryService)
        {
            _ingredientsService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _ingredientsService.GetAll();
            if (data.IsSuccessed)
            {
                return Ok(data);

            }
            else { return BadRequest(); }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _ingredientsService.GetById( id);
            if (data.IsSuccessed)
            {
                return Ok(data);

            }
            else { return BadRequest(); }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IngredientCreateRequest request)
        {
            var data = await _ingredientsService.CreateIngredient(request);
            var ingredientId = data.ResultObj;
            if (ingredientId == 0)
                return BadRequest();

            var ingredient = await _ingredientsService.GetById(ingredientId);

            return CreatedAtAction(nameof(GetById), new { id = ingredientId }, ingredient);
        }
        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] IngredienUpdateRequest request)
        {
            var data = await _ingredientsService.UpdateIngredient(request);
            if(data.IsSuccessed)
            {
                return Ok(data);

            }
            else { return BadRequest(data.Message   ); }

        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int ingredientId)
        {
            var data = await _ingredientsService.DeleteIngredient(ingredientId);
            if (data > 0)
            {
                return Ok();

            }
            else { return BadRequest(); }
        }
    }
}