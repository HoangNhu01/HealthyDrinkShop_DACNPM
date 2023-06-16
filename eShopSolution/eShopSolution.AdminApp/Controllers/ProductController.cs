using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eShopSolution.AdminApp.Hubs;
using eShopSolution.AdminApp.Services;
using eShopSolution.Utilities.Constants;
using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.Ingredients;
using eShopSolution.ViewModels.Catalog.ProductImages;
using eShopSolution.ViewModels.Catalog.Products;
using eShopSolution.ViewModels.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Newtonsoft.Json;

namespace eShopSolution.AdminApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        private readonly IConfiguration _configuration;
        private readonly IIngredientApiClient _ingredientApiClient;
        private readonly ICategoryApiClient _categoryApiClient;
        private IHubContext<ProductHub> _hubContext;

        public ProductController(IProductApiClient productApiClient,
            IConfiguration configuration,
            ICategoryApiClient categoryApiClient,
            IIngredientApiClient ingredientApiClient,
            IHubContext<ProductHub> hubContext)
        {
            _configuration = configuration;
            _productApiClient = productApiClient;
            _categoryApiClient = categoryApiClient;
            _ingredientApiClient = ingredientApiClient;
            _hubContext = hubContext;
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
            //ViewBag.Keyword = keyword;

            //var categories = await _categoryApiClient.GetAll(languageId);
            //ViewBag.Categories = categories.ResultObj.Select(x => new SelectListItem()
            //{
            //    Text = x.Name,
            //    Value = x.Id.ToString(),
            //    Selected = categoryId.Contains(x.Id)
            //});
            //if (categoryId.Length > 0 && categoryId != null)
            //{
            //    var currentId = categories.ResultObj.FirstOrDefault(x => categoryId.ToList().Contains(x.Id)).Name;

            //    //ViewBag.CurrentCateId = currentId;
            //}
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var productCreateRequest = new ProductCreateRequest();

            var ingredients = await _ingredientApiClient.GetAll();

            foreach (var ingredient in ingredients.ResultObj)
            {
                productCreateRequest.SelectIngredients.Ingredients.Add(new SelectItem()
                {
                    Id = ingredient.Id.ToString(),
                    Name = ingredient.Name,
                    Selected = false
                });
            }

            var categories = await _categoryApiClient.GetAll(languageId);

            foreach (var category in categories.ResultObj)
            {
                productCreateRequest.SelectCategories.Categories.Add(new SelectItem()
                {
                    Id = category.Id.ToString(),
                    Name = category.Name,
                    Selected = false
                });
            }
            return View(productCreateRequest);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create( ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _productApiClient.CreateProduct(request);
            if (result.IsSuccessed)
            {
                var resultIngreAssign = await _productApiClient.IngredientAssign(result.ResultObj.Id, request.SelectIngredients);
                if (!resultIngreAssign.IsSuccessed)
                {
                    TempData["result"] = "Không thêm được nguyên liệu cho sản phẩm mới";
                }
                var resultCateAssign = await _productApiClient.CategoryAssign(result.ResultObj.Id, request.SelectCategories);
                if (!resultCateAssign.IsSuccessed)
                {
                    TempData["result"] = "KHông thêm được danh mục cho sản phảm mới";
                }
                TempData["result"] = result.Message;
                await _hubContext.Clients.All.SendAsync("SendMessage", result.ResultObj, request.SelectIngredients);

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
            var productCategory = productObj.ResultObj
                                            .ProductInCategories
                                            .Select(x => new CategoryVm()
                                            {
                                                Id = x.Category.Id,
                                                ParentId = x.Category.ParentId,
                                                Name = x.Category.CategoryTranslations
                                                                 .FirstOrDefault(x => x.LanguageId == languageId).Name,
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
            var productIngredient = productObj.ResultObj
                                              .IngredientInProducts.Select(x => new IngredientVm()
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

            var product = _productApiClient.GetById(id, languageId).Result.ResultObj;
            var editVm = new ProductUpdateRequest()
            {
                Id = product.Id,
                Description = product.Description,
                Details = product.Details,
                Name = product.Name,
                SeoAlias = product.SeoAlias,
                SeoDescription = product.SeoDescription,
                SeoTitle = product.SeoTitle,
                Price = product.Price,
                Stock = product.Stock,
            };
            return View(editVm);
        }
      
        [HttpPost]
        //[Consumes("multipart/form-data")]
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
        public IActionResult Detail(int id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var product = _productApiClient.GetById(id, languageId).Result.ResultObj;
            return View(product);
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
            if (result.ResultObj)
            {
                TempData["result"] = "Xóa sản phẩm thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Xóa không thành công");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddImage(int id)
        {
            ViewBag.ProductId = id;
            return View(new ProductImageCreateRequest());
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddImage(int id, ProductImageCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _productApiClient.AddImage(id, request);
            if (result)
            {
                TempData["result"] = "Thêm ảnh thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Thêm ảnh thất bại");
            ViewBag.ProductId = id;
            return View(request);
        }
        [HttpGet]
        public IActionResult DeleteImage(int id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var product = _productApiClient.GetById(id, languageId).Result.ResultObj;
            var productImage = product.ProductImages.Where(x =>!x.IsDefault).Select(x => new ProductImageVm()
            {
                ProductId = x.ProductId,
                Id = x.Id,
                Data = x.Data,
                SortOrder = x.SortOrder,
                IsDefault = x.IsDefault
            }).ToArray().OrderByDescending(x => x.SortOrder);
            ViewBag.Id = id;
            return View(productImage);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(ProductImageVm request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _productApiClient.DeleteImage(request.Id);
            if (result.IsSuccessed)
            {
                TempData["result"] = $"Xóa ảnh số {request.Id} thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Xóa không thành công");
            return RedirectToAction("Index");
        }
    }

}