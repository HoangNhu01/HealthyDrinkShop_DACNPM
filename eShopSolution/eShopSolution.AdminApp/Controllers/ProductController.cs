using System.Linq;
using System.Threading.Tasks;
using eShopSolution.AdminApp.Services;
using eShopSolution.Utilities.Constants;
using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.Ingredients;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace eShopSolution.AdminApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        private readonly IConfiguration _configuration;
        private readonly IIngredientApiClient _ingredientApiClient;
        private readonly ICategoryApiClient _categoryApiClient;

        public ProductController(IProductApiClient productApiClient,
            IConfiguration configuration,
            ICategoryApiClient categoryApiClient,
            IIngredientApiClient ingredientApiClient)
        {
            _configuration = configuration;
            _productApiClient = productApiClient;
            _categoryApiClient = categoryApiClient;
            _ingredientApiClient = ingredientApiClient;
        }

        public async Task<IActionResult> Index(string keyword, int[]? categoryId, int pageIndex = 1, int pageSize = 10)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var request = new GetManageProductPagingRequest()
            {
                Keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize,
                LanguageId = languageId,
                CategoryIds = categoryId.ToList()
            };
            var data = await _productApiClient.GetPagings(request);
            ViewBag.Keyword = keyword;

            var categories = await _categoryApiClient.GetAll(languageId);
            //ViewBag.Categories = categories.Select(x => new SelectListItem()
            //{
            //    Text = x.Name,
            //    Value = x.Id.ToString(),
            //    Selected = categoryId.HasValue && categoryId.Value == x.Id
            //});

            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _productApiClient.CreateProduct(request);
            if (result)
            {
                TempData["result"] = "Thêm mới sản phẩm thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Thêm sản phẩm thất bại");
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> CategoryAssign(int id)
        {
            var roleAssignRequest = await GetCategoryAssignRequest(id);
            return View(roleAssignRequest);
        }

        [HttpPost]
        public async Task<IActionResult> CategoryAssign(CategoryAssignRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _productApiClient.CategoryAssign(request.Id, request);

            if (result.IsSuccessed)
            {
                TempData["result"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            var categoryAssignRequest = await GetCategoryAssignRequest(request.Id);

            return View(categoryAssignRequest);
        }

        private async Task<CategoryAssignRequest> GetCategoryAssignRequest(int id)
        {
            
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var productObj = await _productApiClient.GetById(id, languageId);
            var productCategory = productObj.ProductInCategories.Select(x => new CategoryVm()
            {
                Id = x.Category.Id,
                ParentId = x.Category.ParentId,
                Name = x.Category.CategoryTranslations.FirstOrDefault(x => x.LanguageId == languageId).Name,
            });
            var categories = await _categoryApiClient.GetAll(languageId);
            var categoryAssignRequest = new CategoryAssignRequest();
            foreach (var category in categories.ResultObj)
            {
                categoryAssignRequest.Categories.Add(new SelectItem()
                {
                    Id = category.Id.ToString(),
                    Name = category.Name,
                    Selected = productCategory.FirstOrDefault(x =>x.Id == category.Id) != null
                });
            }
            return categoryAssignRequest;
        }

        [HttpGet]
        public async Task<IActionResult> IngredientAssign(int id)
        {
            var ingredientAssignRequest = await GetIngredientAssignRequest(id);
            return View(ingredientAssignRequest);
        }

        [HttpPost]
        public async Task<IActionResult> IngredientAssign(IngredientAssignRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _productApiClient.IngredientAssign(request.Id, request);

            if (result.IsSuccessed)
            {
                TempData["result"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            var ingredientAssignRequest = await GetIngredientAssignRequest(request.Id);

            return View(ingredientAssignRequest);
        }

        private async Task<IngredientAssignRequest> GetIngredientAssignRequest(int id)
        {

            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var productObj = await _productApiClient.GetById(id, languageId);
            var productIngredient = productObj.IngredientInProducts.Select(x => new IngredientVm()
            {
                Id = x.Ingredient.Id,
                Description = x.Ingredient.Description,
                Name = x.Ingredient.Name,
                Stock = x.Ingredient.Stock
            });
            var ingredients = await _ingredientApiClient.GetAll();
            var ingredientAssignRequest = new IngredientAssignRequest();
            foreach (var ingredient in ingredients.ResultObj)
            {
                ingredientAssignRequest.Ingredients.Add(new SelectItem()
                {
                    Id = ingredient.Id.ToString(),
                    Name = ingredient.Name,
                    Selected = productIngredient.FirstOrDefault(x => x.Id == ingredient.Id) != null
                });
            }
            return ingredientAssignRequest;
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var product = await _productApiClient.GetById(id, languageId);
            var editVm = new ProductUpdateRequest()
            {
                Id = product.Id,
                Description = product.Description,
                Details = product.Details,
                Name = product.Name,
                SeoAlias = product.SeoAlias,
                SeoDescription = product.SeoDescription,
                SeoTitle = product.SeoTitle
            };
            return View(editVm);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Edit([FromForm] ProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _productApiClient.UpdateProduct(request);
            if (result)
            {
                TempData["result"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Cập nhật sản phẩm thất bại");
            return View(request);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return View(new ProductVm()
            {
                Id = id
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ProductVm request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _productApiClient.DeleteProduct(request.Id);
            if (result)
            {
                TempData["result"] = "Xóa sản phẩm thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Xóa không thành công");
            return View(request);
        }
    }
}