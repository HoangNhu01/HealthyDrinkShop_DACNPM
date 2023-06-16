using eShopSolution.AdminApp.Services;
using eShopSolution.Data.Enums;
using eShopSolution.ViewModels.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderApiClient _orderApiClient;

        public OrderController(
            IConfiguration configuration,
            IOrderApiClient orderApiClient)
        {
            _configuration = configuration;
            _orderApiClient = orderApiClient;

        }
        // GET: OrderController
        public async Task<IActionResult> Index(string keyword)
        {
            var result = await _orderApiClient.GetAll(keyword);
            ViewBag.Status = Enum.GetValues(typeof(OrderStatus));
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(result.ResultObj);
        }

        // GET: OrderController/Details/5
        public async Task<IActionResult> Detail(Guid id)
        {
            var result = await _orderApiClient.GetById(id);
            if (TempData["result"] != null)
            {
                ViewBag.SuccessMsg = TempData["result"];
            }
            return View(result.ResultObj);
        }

        // GET: OrderController/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var result = await _orderApiClient.GetById(id);
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(OrderStatus)));
            return View(result.ResultObj);
        }

        // POST: OrderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, OrderStatus collection)
        {
            try
            {
                var result = await _orderApiClient.UpdateOrderStatus(id, collection);
                if (result.ResultObj)
                {
                    TempData["result"] = "Cập nhật trạng thái đơn hàng thành công";
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: OrderController/Delete/5
        public ActionResult Delete(Guid id)
        {
            var orderDelete = new OrderVm()
            {
                Id = id,
            };
            return View(orderDelete);
        }

        // POST: OrderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(OrderVm orderVm)
        {
            try
            {
                var result = await _orderApiClient.DeleteOrder(orderVm.Id);
                if (result.IsSuccessed)
                {
                    TempData["result"] = "Cập nhật trạng thái đơn hàng thành công";
                    return RedirectToAction("Index");
                }
                return View(orderVm);

            }
            catch
            {
                return View();
            }
        }
    }
}
