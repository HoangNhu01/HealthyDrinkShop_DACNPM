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
using eShopSolution.AdminApp.IpAddresss;
using eShopSolution.ViewModels.Sales;
using Microsoft.EntityFrameworkCore.Internal;
using System.Data.Common;

namespace eShopSolution.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : Controller
    {
        private readonly ILogger<PaymentsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IIpAdrress _ipAdrress;


        public PaymentsController(ILogger<PaymentsController> logger, IConfiguration configuration, IIpAdrress ipAdrress)
        {
            _logger = logger;
            _configuration = configuration;
            _ipAdrress = ipAdrress;

        }
        [HttpPost]
        public ActionResult PaymentRequest(/*[FromBody]CheckOutRequest checkOutRequest*/)
        {
            string url = _configuration[SystemConstants.PaymentApi.Url];
            string returnUrl = _configuration[SystemConstants.PaymentApi.ReturnUrl];
            string tmnCode = _configuration[SystemConstants.PaymentApi.TmnCode];
            string hashSecret = _configuration[SystemConstants.PaymentApi.HashSecret];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.1.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", "10000000" /*(checkOutRequest.TotalPrice * 100).ToString()*/); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", _ipAdrress.GetLocalIPAddress().ResultObj); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "8d36dc87-1edc-4d20-ae3e-8b89646c6c56"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Ok(new {data = paymentUrl});
        }
       
    }
}
