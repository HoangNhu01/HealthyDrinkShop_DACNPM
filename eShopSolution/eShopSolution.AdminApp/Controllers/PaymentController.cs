using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using eShopSolution.Utilities.Constants;
using eShopSolution.Utilities.PaymentTool;
using eShopSolution.ViewModels.Sales;
using Microsoft.EntityFrameworkCore.Internal;
using System.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using eShopSolution.AdminApp.Hubs;
using Azure.Core;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using eShopSolution.AdminApp.Models;
using eShopSolution.ViewModels.Common;

namespace eShopSolution.WebApp.Controllers
{
  
    public class PaymentController : Controller
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IConfiguration _configuration;
        private IHubContext<OrderHub> _hubContext;


        public PaymentController(ILogger<PaymentController> logger, IConfiguration configuration, IHubContext<OrderHub> hubContext)
        {
            _logger = logger;
            _configuration = configuration;
            _hubContext = hubContext;

        }
        [AllowAnonymous]
        //[HttpGet("payment-confirm")]
        public async Task<ActionResult> PaymentConfirm()
        {
            var payMentStatus = new ApiResult<string>();
            
            if (Request.Query.Count > 0)
            {
                string hashSecret = _configuration[SystemConstants.PaymentApi.HashSecret];

                var vnpayData = Request.Query.ToHashSet();
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (var s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s.Key) && s.Key.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s.Key, s.Value);
                    }
                }
                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.Query["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        payMentStatus.ResultObj = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                        payMentStatus.IsSuccessed = true;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        payMentStatus.ResultObj = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                        payMentStatus.IsSuccessed = false;
                    }
                }
                else
                {
                    payMentStatus.ResultObj = "Có lỗi xảy ra trong quá trình xử lý";
                    payMentStatus.IsSuccessed= false;
                }

            }
            else
            {
                payMentStatus.ResultObj = "Không tìm thấy yêu cầu thanh toán";
                payMentStatus.IsSuccessed = false;
            }
            await _hubContext.Clients.All.SendAsync("StatusOrderMessage", payMentStatus);
            
            return View(payMentStatus);
        }
    }
}
