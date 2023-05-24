using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopSolution.Application.Catalog.Products;
using eShopSolution.Utilities.Constants;
using eShopSolution.Utilities.Exceptions;
using eShopSolution.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace eShopSolution.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMemoryCache _cache;
        public OrdersController(IProductService productService, IMemoryCache cache)
        {
            _productService = productService;
            _cache = cache;
        }
        [HttpPost("{id}/{languageId}")]
        public async Task<IActionResult> AddToCart(int id, string languageId)
        {
            var product = await _productService.GetById(id, languageId);
            var cartItems = _cache.Get<List<CartItemViewModel>>(SystemConstants.CartCaching) 
                                        ?? new List<CartItemViewModel>();
            //var session = HttpContext.Session.GetString(SystemConstants.CartSession);
            //List<CartItemViewModel> currentCart = new List<CartItemViewModel>();
            //if (session != null)
            //    currentCart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(session);

            int quantity = 1;
            if (cartItems.Any(x => x.ProductId == id))
            {
                quantity = cartItems.First(x => x.ProductId == id).Quantity + 1;
            }

            var cartItem = new CartItemViewModel()
            {
                ProductId = id,
                Description = product.Description,
                Image = product.ThumbnailImage,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            };

            cartItems.Add(cartItem);
            _cache.Set(SystemConstants.CartCaching, cartItems);

            //HttpContext.Session.SetString(SystemConstants.CartSession, JsonConvert.SerializeObject(currentCart));
            return Ok(cartItems);
        }

        [HttpPatch("{productId}/{quantity}")]
        public IActionResult RemoveFromCart(int productId, int quantity)
        {
            var cartItems = _cache.Get<List<CartItemViewModel>>(SystemConstants.CartCaching);
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
            _cache.Set(SystemConstants.CartCaching, cartItems);
            return Ok(cartItems);
        }
        [HttpGet]
        public IActionResult ViewCart()
        {
            // Get the cart items from the cache or return an empty cart
            var cartItems = _cache.Get<List<CartItemViewModel>>(SystemConstants.CartCaching) ?? new List<CartItemViewModel>();
            return Ok();
        }
    }
}
