using eShopSolution.Data.EF;
using eShopSolution.ViewModels.Catalog.Categories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using eShopSolution.ViewModels.Common;
using eShopSolution.Data.Entities;
using eShopSolution.Utilities.Exceptions;
using eShopSolution.Application.Common;
using eShopSolution.ViewModels.Catalog.Products;

namespace eShopSolution.Application.Catalog.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly EShopDbContext _context;

        public CategoryService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(CategoryCreateRequest request)
        {
            var category = new Category()
            {
                IsShowOnHome = true,
                ParentId = request.ParentId,
                SortOrder = request.SortOrder,
                Status = request.Status,
                CategoryTranslations = new List<CategoryTranslation>()
                {
                    new CategoryTranslation()
                    {
                        LanguageId = request.LanguageId,
                        SeoAlias = request.SeoAlias,
                        SeoDescription = request.SeoDescription,
                        SeoTitle = request.SeoTitle,
                        Name = request.Name,                        
                    }
                }
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category.Id;
        }

        public async Task<int> Delete(int categoryId)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null) throw new EShopException($"Cannot find a category: {categoryId}");
            _context.Categories.Remove(category);
            return await _context.SaveChangesAsync();
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
                    ParentId = x.Category.ParentId,
                    SeoAlias = x.SeoAlias,
                    SeoDescription = x.SeoDescription,
                    SeoTitle= x.SeoTitle,
                    Status = x.Category.Status,
                    SortOrder = x.Category.SortOrder,
                    LanguageId = languageId,

                }).ToListAsync()
            };
        }
        

        public async Task<ApiResult<CategoryVm>> GetById(string languageId, int id)
        {
            var query = _context.CategoryTranslations.Include(x => x.Category).ThenInclude(x =>x.ProductInCategories).ThenInclude(x => x.Product)
                                                     .FirstOrDefault(x => x.Id == id && x.LanguageId == languageId);
            //var query = from c in _context.Categories
            //            join ct in _context.CategoryTranslations on c.Id equals ct.CategoryId
            //            where ct.LanguageId == languageId && c.Id == id
            //            select new { c, ct };
            return new ApiSuccessResult<CategoryVm>()
            {
                ResultObj = new CategoryVm()
                {
                    Id = query.Id,
                    Name = query.Name,
                    SeoDescription= query.SeoDescription,
                    SeoAlias = query.SeoAlias,
                    SeoTitle = query.SeoTitle,
                    Status = query.Category.Status,
                    SortOrder= query.Category.SortOrder,
                    ParentId = query.Category.ParentId,
                    ProductInCategories = query.Category.ProductInCategories                   
                }
            };
        }

        public async Task<int> Update(CategoryUpdateRequest request)
        {
            var category = _context.CategoryTranslations.Include(x => x.Category).FirstOrDefault(x => x.LanguageId == request.LanguageId
                                                                                            && x.CategoryId == request.Id);
            if (category == null) throw new EShopException($"Cannot find a product with id: {request.Id}");

            category.SeoDescription = request.SeoDescription;
            category.SeoTitle = request.SeoTitle;
            category.SeoAlias = request.SeoAlias;
            category.Name = request.Name;
            var rs = await _context.SaveChangesAsync();
            return rs;
        }
    }
}