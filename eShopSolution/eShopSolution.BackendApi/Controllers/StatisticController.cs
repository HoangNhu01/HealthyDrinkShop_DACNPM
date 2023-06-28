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
    }
}
