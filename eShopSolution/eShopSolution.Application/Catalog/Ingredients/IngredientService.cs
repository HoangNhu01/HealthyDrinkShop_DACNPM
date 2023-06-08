using eShopSolution.Data.EF;
using eShopSolution.ViewModels.Catalog.Ingredients;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using eShopSolution.ViewModels.Common;
using eShopSolution.Data.Entities;
using eShopSolution.Utilities.Exceptions;

namespace eShopSolution.Application.Catalog.Ingredients
{
    public class IngredientService : IIngredientsService
    {
        private readonly EShopDbContext _context;

        public IngredientService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<List<IngredientVm>>> GetAll()
        {
            var query = _context.Ingredients.Include(x => x.IngredientInProducts).ThenInclude(x => x.Product);
           
            return new ApiSuccessResult<List<IngredientVm>>()
            {
                ResultObj = await query.Select(x => new IngredientVm()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    Stock = x.Stock,
                }).ToListAsync()
            };
        }
        

        public async Task<ApiResult<IngredientVm>> GetById(int id)
        {
            var query = await _context.Ingredients.Include(x => x.IngredientInProducts).ThenInclude(x => x.Product)
                                            .FirstOrDefaultAsync(x => x.Id == id);

            return new ApiSuccessResult<IngredientVm>()
            {
                ResultObj = new IngredientVm()
                {
                    Id = query.Id,
                    Name = query.Name,
                    Description = query.Description,
                    Price = query.Price,
                    Stock = query.Stock,
                }
            };
        }

        public async Task<ApiResult<int>> CreateIngredient(IngredientCreateRequest request)
        {
            var ingredient = new Ingredient()
            {
                Name = request.Name,
                Stock = request.Stock,
                Description = request.Description,
                Price = request.Price,
                
            };
            await _context.Ingredients.AddAsync(ingredient);
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<int>(ingredient.Id);
        }
        public async Task<ApiResult<bool>> UpdateIngredient(IngredienUpdateRequest request)
        {
            var ingredient = await _context.Ingredients.FindAsync(request.Id);
            if (ingredient == null)
                throw new EShopException("Không tìm thấy nguyên liệu");
            ingredient.Description = request.Description;
            ingredient.Price = request.Price;
            ingredient.Stock = request.Stock;
            ingredient.Name = request.Name;
           
            await _context.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true,"Cập nhật thành công");
        }
        public async Task<int> DeleteIngredient(int ingredientId)
        {
            var ingredient = await _context.Ingredients.FindAsync(ingredientId);
            if (ingredient == null) throw new EShopException($"Cannot find a ingredient: {ingredientId}");
            _context.Ingredients.Remove(ingredient);
            return await _context.SaveChangesAsync();
        }

    }
}