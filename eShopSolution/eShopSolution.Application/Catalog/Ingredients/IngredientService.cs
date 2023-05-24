using eShopSolution.Data.EF;
using eShopSolution.ViewModels.Catalog.Ingredients;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using eShopSolution.ViewModels.Common;

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
           
            return new ApiResult<List<IngredientVm>>()
            {
                ResultObj = await query.Select(x => new IngredientVm()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                }).ToListAsync()
            };
        }
        

        public async Task<ApiResult<IngredientVm>> GetById(int id)
        {
            var query = _context.Ingredients.Include(x => x.IngredientInProducts).ThenInclude(x => x.Product)
                                            .FirstOrDefault(x => x.Id == id);

            return new ApiResult<IngredientVm>()
            {
                ResultObj = new IngredientVm()
                {
                    Id = query.Id,
                    Name = query.Name,
                    Description = query.Description 
                }
            };
        }

       
    }
}