using eShopSolution.Application.Catalog.Ingredients;
using eShopSolution.Application.Catalog.Statistic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.Threading.Tasks;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : Controller
    {
        private readonly IStatisticService _statisticService;

        public StatisticController (
            IStatisticService statisticService)
        {
           _statisticService = statisticService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var rs = await _statisticService.GetRevenueByMonth();
            return Ok(rs);
        }
        [HttpGet("percentages-of-category")]
        public async Task<IActionResult> GetPercentagesOfCategory()
        {
            var rs = await _statisticService.GetPercentagesOfCategory();
            return Ok(rs);
        }
        [HttpGet("percentages-of-all")]
        public async Task<IActionResult> GetPercentagesOfAll()
        {
            var rs = await _statisticService.GetOrderStatistic();
            return Ok(rs);
        } 
        [HttpGet("user-order")]
        public async Task<IActionResult> GetUserOrder()
        {
            var rs = await _statisticService.GetUserOrder();
            return Ok(rs);
        } 
        [HttpGet("product-order")]
        public async Task<IActionResult> GetProductOrder()
        {
            var rs = await _statisticService.GetProductOrder();
            return Ok(rs);
        }
    }
}
