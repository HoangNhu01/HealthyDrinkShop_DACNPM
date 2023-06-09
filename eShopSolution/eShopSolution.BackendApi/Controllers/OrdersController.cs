using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopSolution.AdminApp.IpAddresss;
using eShopSolution.Application.Catalog.Products;
using eShopSolution.Application.Sales.Orders;
using eShopSolution.Application.System.Users;
using eShopSolution.Data.Entities;
using eShopSolution.Utilities.Constants;
using eShopSolution.Utilities.Exceptions;
using eShopSolution.ViewModels.Sales;
using eShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        //private readonly IMemoryCache _cache;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IIpAdrress _ipAdrress;


        public OrdersController(IProductService productService, 
                                IConnectionMultiplexer redisConnection,
                                IUserService userService, IOrderService orderService,
                                IIpAdrress ipAdrress)
        {
            _productService = productService;
            //_cache = cache;
            _redisConnection = redisConnection;
            _userService = userService;
            _orderService = orderService;
            _ipAdrress = ipAdrress; 
        }
        [HttpPost("{id}/{languageId}")]
        public async Task<IActionResult> AddToCart(int id, string languageId, int clientQuantity)
        {
            var product =  _productService.GetById(id, languageId).Result.ResultObj;
            var ipAdd = _ipAdrress.GetLocalIPAddress();
            string cartKey = $"cart:{ipAdd.ResultObj}";

            IDatabase redisDb = _redisConnection.GetDatabase();
            string cartJson = await redisDb.StringGetAsync(cartKey);

            var cartItems = cartJson != null ? JsonConvert.DeserializeObject<List<CartItemVm>>(cartJson) 
                                          : new List<CartItemVm>();

            int quantity = clientQuantity;
            if (cartItems.Any(x => x.ProductId == id))
            {
                var cart = cartItems.First(x => x.ProductId == id);
                quantity = cart.Quantity + clientQuantity;
                cartItems.Remove(cart);
            }

            var cartItem = new CartItemVm()
            {
                ProductId = id,
                Description = product.Description,
                Image = product.ThumbnailImage,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            };
            cartItems.Add(cartItem);
            string cartJsonConvert = JsonConvert.SerializeObject(cartItems);
            await redisDb.StringSetAsync(cartKey, cartJsonConvert);
            return Ok(cartItems);
        }
        
        [HttpPatch("{productId}/{quantity}")]
        public async Task<IActionResult> RemoveFromCart(int productId, int quantity)
        {
            var useClaims = HttpContext.User.Identity.Name;
            var ipAdd = _ipAdrress.GetLocalIPAddress();
            string cartKey = $"cart:{ipAdd.ResultObj}";

            IDatabase redisDb = _redisConnection.GetDatabase();
            string cartJson = await redisDb.StringGetAsync(cartKey);

            var cartItems = JsonConvert.DeserializeObject<List<CartItemVm>>(cartJson);
                                         
            if (cartItems == null)
            {
                throw new EShopException("Dont have cart in cache memory");
            }
            foreach (var item in cartItems)
            {
                if (item.ProductId == productId)
                {
                    if (quantity == 0)
                    {
                        cartItems.Remove(item);
                        break;
                    }
                    item.Quantity = quantity;
                }
            }
            string cartJsonConvert = JsonConvert.SerializeObject(cartItems);
            await redisDb.StringSetAsync(cartKey, cartJsonConvert);
            return Ok(cartItems);
        }
        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {
            var useClaims = HttpContext.User.Identity.Name;
            var ipAdd = _ipAdrress.GetLocalIPAddress();
            string cartKey = $"cart:{ipAdd.ResultObj}";

            IDatabase redisDb = _redisConnection.GetDatabase();
            string cartJson = await redisDb.StringGetAsync(cartKey);

            var cartItems = cartJson != null ? JsonConvert.DeserializeObject<List<CartItemVm>>(cartJson)
                                          : new List<CartItemVm>();
            return Ok(cartItems);
        }
        [Authorize]
        [HttpPut]
        public IActionResult MergingCart()
        {

            return Ok();
        }
        
        [HttpPost("checkouts")]
        public async Task<IActionResult> CheckOutCart([FromBody] CheckOutRequest checkOutRequest) 
        {
            var useClaims = HttpContext.User.Identity.Name;
            var ipAdd = _ipAdrress.GetLocalIPAddress();
            string cartKey = $"cart:{ipAdd.ResultObj}";

            IDatabase redisDb = _redisConnection.GetDatabase();
            string cartJson = await redisDb.StringGetAsync(cartKey);

            var cartItems = JsonConvert.DeserializeObject<List<CartItemVm>>(cartJson);
            checkOutRequest.CartItems = cartItems;
            var data = await _orderService.Create(checkOutRequest);
            if (data.ResultObj > 0)
            {
                
                await redisDb.StringGetDeleteAsync(cartKey);
                return Ok(checkOutRequest);
            }
            return BadRequest();
        }
    }
}
