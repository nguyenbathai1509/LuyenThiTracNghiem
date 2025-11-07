using System;
using System.Linq;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VNPAY.NET;
using VNPAY.NET.Models;
using VNPAY.NET.Enums;

namespace LuyenThiTracNghiem.Controllers
{
    public class PaymentController : Controller
    {
        private readonly DataContext _context;
        private readonly IVnpay _vnpay;
        private readonly IConfiguration _configuration;

        public PaymentController(DataContext context, IVnpay vnpay, IConfiguration configuration)
        {
            _context = context;
            _vnpay = vnpay;
            _configuration = configuration;

            _vnpay.Initialize(
                _configuration["Vnpay:TmnCode"]!,
                _configuration["Vnpay:HashSecret"]!,
                _configuration["Vnpay:BaseUrl"]!,
                _configuration["Vnpay:ReturnUrl"]!
            );
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Payment/CreatePaymentUrl")]
        public IActionResult CreatePaymentUrl(double amount = 10000)
        {
            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

                var payment = new tblPayment
                {
                    UserId = HttpContext.Session.GetInt32("UserId") ?? 0,
                    Amount = (decimal)amount,
                    PaymentMethod = "VNPAY",
                    PaymentStatus = "Pending",
                    PaymentDate = DateTime.Now,
                    Note = "Thanh toán thử sandbox",
                    CreatedBy = HttpContext.Session.GetString("FullName") ?? "Người dùng"
                };

                _context.Payments.Add(payment);
                _context.SaveChanges();

                var request = new PaymentRequest
                {
                    PaymentId = payment.PaymentId,
                    Money = amount,
                    Description = payment.Note,
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY,
                    Currency = Currency.VND,
                    Language = DisplayLanguage.Vietnamese,
                    CreatedDate = DateTime.Now
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);

                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi tạo Payment URL: " + ex.Message);
            }
        }

        [HttpGet("Payment/Callback")]
        public IActionResult Callback()
        {
            if (!Request.QueryString.HasValue)
                return NotFound("Không nhận được thông tin thanh toán.");

            try
            {
                var result = _vnpay.GetPaymentResult(Request.Query);

                var payment = _context.Payments.FirstOrDefault(p => p.PaymentId == result.PaymentId);

                if (payment == null)
                {
                    return View("PaymentResult", new tblPayment
                    {
                        PaymentStatus = "Failed",
                        Note = "Không tìm thấy thông tin giao dịch."
                    });
                }

                if (payment.IsProcessed)
                {
                    return View("PaymentResult", payment);
                }

                if (result.IsSuccess)
                {
                    payment.PaymentStatus = "Success";
                    payment.TransactionCode = result.VnpayTransactionId.ToString();
                    payment.PaymentDate = DateTime.Now;
                    payment.UpdatedBy = HttpContext.Session.GetString("FullName") ?? "Người dùng";
                    payment.UpdatedDate = DateTime.Now;

                    var user = _context.Users.FirstOrDefault(u => u.UserId == payment.UserId);
                    if (user != null)
                    {
                        user.Balance += payment.Amount;
                        user.UpdatedAt = DateTime.Now;
                        user.UpdatedBy = payment.UpdatedBy;
                    }

                    payment.IsProcessed = true;
                    _context.SaveChanges();
                }
                else
                {
                    payment.PaymentStatus = "Failed";
                    _context.SaveChanges();
                }

                return View("PaymentResult", payment);
            }
            catch (Exception ex)
            {
                return View("PaymentResult", new tblPayment
                {
                    PaymentStatus = "Failed",
                    Note = "Lỗi xử lý callback: " + ex.Message
                });
            }
        }
    }
}
