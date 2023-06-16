using eShopSolution.AdminApp.Models;
using eShopSolution.AdminApp.Services;
using eShopSolution.Utilities.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace eShopSolution.AdminApp.Controllers.Components
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly ILanguageApiClient _languageApiClient;
        private readonly IOrderApiClient _orderApiClient;

        public NavigationViewComponent(ILanguageApiClient languageApiClient, IOrderApiClient orderApiClient)
        {
            _languageApiClient = languageApiClient;
            _orderApiClient = orderApiClient;   
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var languages = await _languageApiClient.GetAll();
            var navigationVm = new NavigationViewModel()
            {
                CurrentLanguageId = HttpContext
                .Session
                .GetString(SystemConstants.AppSettings.DefaultLanguageId),
                Languages = languages.ResultObj
            };
            ViewBag.NavigationVm = _orderApiClient.GetAll(null).Result.ResultObj.Take(4);
            return View("Default", navigationVm);
        }
    }
}