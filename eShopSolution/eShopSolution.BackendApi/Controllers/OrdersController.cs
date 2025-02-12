﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShopSolution.AdminApp.IpAddresss;
using eShopSolution.Application.Catalog.Products;
using eShopSolution.Application.Sales.Orders;
using eShopSolution.Application.AppSystem.Users;
using eShopSolution.BackendApi.Hubs;
using eShopSolution.Data.Entities;
using eShopSolution.Data.Enums;
using eShopSolution.Utilities.Constants;
using eShopSolution.Utilities.Exceptions;
using eShopSolution.ViewModels.Sales;
using eShopSolution.ViewModels.AppSystem.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StackExchange.Redis;
using QueueEngine.Interfaces;
using QueueEngine.Models.QueueData;

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
        private IQueueService<OrderQueue> _queueService;
        private IHubContext<OrderHub> _hubContext;


        public OrdersController(IProductService productService, 
                                IConnectionMultiplexer redisConnection,
                                IUserService userService, IOrderService orderService,
                                IQueueService<OrderQueue> queueService,
                                IIpAdrress ipAdrress, IHubContext<OrderHub> hubContext)
        {
            _productService = productService;
            //_cache = cache;
            _redisConnection = redisConnection;
            _userService = userService;
            _orderService = orderService;
            _ipAdrress = ipAdrress; 
            _queueService = queueService;
            _hubContext = hubContext;
        }
        [HttpPost("{id}/{languageId}/{clientQuantity}")]
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
            var result = await redisDb.StringSetAsync(cartKey, cartJsonConvert);
            if (result)
            {
                await _productService.AddViewcount(cartItem.ProductId);
            }
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
        //[Authorize]
        //[HttpPut]
        //public IActionResult MergingCart()
        //{

        //    return Ok();
        //}
        
        [HttpPost("checkouts")]
        public async Task<IActionResult> CheckOutCart([FromBody] CheckOutRequest checkOutRequest) 
        {
            var ipAdd = _ipAdrress.GetLocalIPAddress();
            string cartKey = $"cart:{ipAdd.ResultObj}";

            IDatabase redisDb = _redisConnection.GetDatabase();
            string cartJson = await redisDb.StringGetAsync(cartKey);

            var cartItems = JsonConvert.DeserializeObject<List<CartItemVm>>(cartJson);
            foreach(var item in checkOutRequest.CartItems)
            {
                var cart = cartItems.FirstOrDefault(x => x.ProductId == item.ProductId);
                if (cart !=null)
                {
                    cartItems.Remove(cart);
                }
            }
            if(checkOutRequest.CartItems == null && checkOutRequest.CartItems.Count == 0)
            {
                checkOutRequest.CartItems = cartItems;
            }
            var data = await _orderService.Create(checkOutRequest);
           
            if (data.IsSuccessed)
            {

                string cartJsonConvert = JsonConvert.SerializeObject(cartItems);
                await redisDb.StringSetAsync(cartKey, cartJsonConvert);
                try
                {
                    _queueService.PushQueue(new OrderQueue
                    {
                        OrderId = data.ResultObj,
                        UserId = checkOutRequest.UserId,
                        Address = checkOutRequest.Address,
                        Email = checkOutRequest.Email,
                        OrderDate = checkOutRequest.OrderDate,
                        PaymentStatus = checkOutRequest.PaymentStatus,
                        CartItems = cartItems,
                        PhoneNumber = checkOutRequest.PhoneNumber,
                        TotalPrice = checkOutRequest.TotalPrice,
                        UserName = checkOutRequest.UserName,
                    });
                }
                catch(EShopException ex)
                {

                }
                await _hubContext.Clients.All.SendAsync("OrderCheckOut", checkOutRequest);

                return Ok(cartItems);
            }
            return BadRequest();
        }
        [Authorize]
        [HttpPut("{orderId}/{orderStatus}")]
        public async Task<IActionResult> UpdateStatus(Guid orderId, OrderStatus orderStatus)
        {
            var result = await _orderService.UpdateStatus(orderId, orderStatus);
            if (result.IsSuccessed)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid orderId)
        {
            var result = await _orderService.Delete(orderId);
            if (result.IsSuccessed)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpGet("customer-order")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var result = await _orderService.GetByUserId(userId);
            if (result.IsSuccessed)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(Guid orderId)
        {
            var result = await _orderService.GetById(orderId);
            if (result.IsSuccessed)
            {
                return Ok(result);
            }
            return BadRequest();
        }
       
    }
}
