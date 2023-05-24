using eShopSolution.Data.EF;
using eShopSolution.ViewModels.Catalog.Categories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using eShopSolution.ViewModels.Common;

namespace eShopSolution.Application.Catalog.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly EShopDbContext _context;

        public CategoryService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<List<CategoryVm>>> GetAll(string languageId)
        {
            var query = _context.CategoryTranslations.Where(x => x.LanguageId == languageId).Include(x => x.Category);
            //var query = from c in _context.Categories
            //            join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
            //            where ct.LanguageId == languageId
            //            select new { c, ct };
            return new ApiResult<List<CategoryVm>>()
            {
                ResultObj = await query.Select(x => new CategoryVm()
                {
                    Id = x.Category.Id,
                    Name = x.Name,
                    ParentId = x.Category.ParentId
                }).ToListAsync()
            };
        }
        

        public async Task<ApiResult<CategoryVm>> GetById(string languageId, int id)
        {
            var query = from c in _context.Categories
                        join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
                        where ct.LanguageId == languageId && c.Id == id
                        select new { c, ct };
            return new ApiResult<CategoryVm>()
            {
                ResultObj = await query.Select(x => new CategoryVm()
                {
                    Id = x.c.Id,
                    Name = x.ct.Name,
                    ParentId = x.c.ParentId
                }).FirstOrDefaultAsync()
            };
        }

       
    }
}