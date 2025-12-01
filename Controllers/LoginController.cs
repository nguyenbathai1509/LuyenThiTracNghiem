using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LuyenThiTracNghiem.Controllers
{
    public class LoginController : Controller
    {
        private readonly DataContext _context;
        public LoginController(DataContext context)
        {
            _context = context;
        }

        private bool IsSafeReturnUrl(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl)) return false;

            if (Url.IsLocalUrl(returnUrl)) return true;

            if (Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri))
            {
                var host = Request.Host.Host;
                var port = Request.Host.Port;
                if (uri.Scheme is "http" or "https"
                    && string.Equals(uri.Host, host, StringComparison.OrdinalIgnoreCase)
                    && (!port.HasValue || uri.Port == port.Value))
                {
                    return true;
                }
            }

            return false;
        }

        [HttpGet("/Login")]
        public IActionResult Index(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public IActionResult Index(tblUser model, string? returnUrl = null)
        {
            var check = _context.Users
                .Where(u => (u.Username == model.Username) && (u.PasswordHash == model.PasswordHash))
                .FirstOrDefault();

            if (check == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                return View(model);
            }

            if (!check.Status)
            {
                ViewBag.Error = "Tài khoản của bạn đã bị khóa hoặc chưa được kích hoạt!";
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", check.UserId);
            HttpContext.Session.SetString("FullName", check.FullName);
            HttpContext.Session.SetString("UserCode", check.UserCode);
            HttpContext.Session.SetString("Username", check.Username);
            HttpContext.Session.SetString("Email", check.Email ?? "");
            HttpContext.Session.SetString("PhoneNumber", check.PhoneNumber ?? "");
            HttpContext.Session.SetInt32("Role", check.Role);

            if (check.Role == 1)
            {
                return Redirect("/Admin");
            }

            if (IsSafeReturnUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
