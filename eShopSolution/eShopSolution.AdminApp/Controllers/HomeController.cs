using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using eShopSolution.AdminApp.Models;
using Microsoft.AspNetCore.Authorization;
using eShopSolution.Utilities.Constants;
using Microsoft.AspNetCore.Http;

namespace eShopSolution.AdminApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var user = User.Identity.Name;
            var img = User.Claims.Select(x => x.Value);
            var jwt = HttpContext.Request.Cookies["Token"];
            if (TempData["img"] != null)
            {
                ViewBag.UserImage = TempData["img"];
            }
            return View();
        }
        public IActionResult FAQ()
        {
            
            if (TempData["img"] != null)
            {
                ViewBag.UserImage = TempData["img"];
            }
            return View();
        }
        public IActionResult Contact()
        {
            
            if (TempData["img"] != null)
            {
                ViewBag.UserImage = TempData["img"];
            }
            return View();
        }
        public IActionResult Profile()
        {
            
            if (TempData["img"] != null)
            {
                ViewBag.UserImage = TempData["img"];
            }
            return View();
        }
         public IActionResult Content()
        {
            
            if (TempData["img"] != null)
            {
                ViewBag.UserImage = TempData["img"];
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult Language(NavigationViewModel viewModel)
        {
            HttpContext.Session.SetString(SystemConstants.AppSettings.DefaultLanguageId,
                viewModel.CurrentLanguageId);

            return RedirectToAction("Index");
        }
    }
}