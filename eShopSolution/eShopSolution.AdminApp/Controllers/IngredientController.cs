using eShopSolution.AdminApp.Services;
using eShopSolution.Data.Enums;
using eShopSolution.Utilities.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using eShopSolution.ViewModels.Catalog.Ingredients;

namespace eShopSolution.AdminApp.Controllers
{
    public class IngredientController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IIngredientApiClient _ingredientApiClient;

        public IngredientController(
            IConfiguration configuration,
            IIngredientApiClient ingredientApiClient)
        {
            _configuration = configuration;
            _ingredientApiClient = ingredientApiClient;

        }
        // GET: CategoryController
        [HttpGet]
        public async Task<IActionResult> Index()
        {         
            var data = await _ingredientApiClient.GetAll();
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(data.ResultObj);
        }

        // GET: CategoryController/Details/5
        public async Task<IActionResult> Detail(int id)
        {         
            var data = await _ingredientApiClient.GetById(id);
            return View(data.ResultObj);
        }

        // GET: CategoryController/Create
        [HttpGet]
        public async Task<ActionResult> Create()
        {           
            return View(new IngredientCreateRequest());
        }

        // POST: CategoryController/Create
        [HttpPost]
        public async Task<IActionResult> Create(IngredientCreateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _ingredientApiClient.CreateIngredient(request);
            if (result.ResultObj)
            {
                TempData["result"] = "Thêm mới nguyên liệu thành công";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Thêm nguyên thất bại");
            return View(request);
        }

        // GET: CategoryController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var ingredient = _ingredientApiClient.GetById(id).Result.ResultObj;
            var editVm = new IngredienUpdateRequest()
            {
                Id = id,
                Name = ingredient.Name,
                Stock = ingredient.Stock,
                Description = ingredient.Description,
                Price = ingredient.Price
            };
            return View(editVm);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IngredienUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _ingredientApiClient.UpdateIngredient(request);
            if (result.IsSuccessed)
            {
                TempData["result"] = "Cập nhật nguyên liệu thành công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Cập nhật nguyên liệu thất bại");
            return View(request);
        }

        // GET: CategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(new IngredientVm()
            {
                Id = id
            });
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(IngredientVm request)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _ingredientApiClient.DeleteIngredient(request.Id);
            if (result.ResultObj)
            {
                TempData["result"] = "Xóa danh nguyên liệu công";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Xóa không thành công");
            return View(request);
        }
    }
}
