using eShopSolution.AdminApp.Services;
using eShopSolution.Data.Enums;
using eShopSolution.Utilities.Constants;
using eShopSolution.ViewModels.Catalog.Categories;
using eShopSolution.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ICategoryApiClient _categoryApiClient;

        public CategoryController(
            IConfiguration configuration,
            ICategoryApiClient categoryApiClient)
        {
            _configuration = configuration;
            _categoryApiClient = categoryApiClient;
           
        }
        // GET: CategoryController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var data = await _categoryApiClient.GetAll(languageId);
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.ResultObj);
        }

        // GET: CategoryController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var data = await _categoryApiClient.GetById(languageId, id);
            return View(data.ResultObj);
        }

        // GET: CategoryController/Create
        [HttpGet("created-categories")]
        public async Task<ActionResult> CreateAsync()
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);
            var data = await _categoryApiClient.GetAll(languageId);
            ViewBag.ParentId = data.ResultObj.Select(x => new SelectListItem(x.Id.ToString(),x.Id.ToString(),true));
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(Status)));
            return View(new CategoryCreateRequest());
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CategoryCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _categoryApiClient.CreateCategory(request);
            if (result.ResultObj)
            {
                TempData["result"] = "Thêm mới danh mục thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Thêm danh mục thất bại");
            return View(request);
        }

        // GET: CategoryController/Edit/5
        public async Task<ActionResult> EditAsync(int id)
        {
            var languageId = HttpContext.Session.GetString(SystemConstants.AppSettings.DefaultLanguageId);

            var category = _categoryApiClient.GetById(languageId, id).Result.ResultObj;
            var editVm = new CategoryUpdateRequest()
            {
                Id = id,
                Name = category.Name,
                SeoAlias = category.SeoAlias,
                SeoDescription = category.SeoDescription,
                SeoTitle = category.SeoTitle,
                LanguageId = languageId,
                ParentId = category.ParentId,
                Status = category.Status,
                SortOrder = category.SortOrder,
            };
            var data = await _categoryApiClient.GetAll(languageId);
            ViewBag.ParentId = data.ResultObj.Select(x => new SelectListItem(x.Id.ToString(), x.Id.ToString(), true));
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(Status)));
            return View(editVm);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( CategoryUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _categoryApiClient.UpdateCategory(request);
            if (result>0)
            {
                TempData["result"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Cập nhật danh mục thất bại");
            return View(request);
        }

        // GET: CategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(new CategoryVm()
            {
                Id = id
            });
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(CategoryVm request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _categoryApiClient.DeleteCategory(request.Id);
            if (result.ResultObj)
            {
                TempData["result"] = "Xóa danh mục thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Xóa không thành công");
            return View(request);
        }
    }
}
