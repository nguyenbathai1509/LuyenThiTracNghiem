using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using LuyenThiTracNghiem.Utilities;

namespace LuyenThiTracNghiem.Controllers
{
    [AllowAnonymous]
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(tblUser model, string? returnUrl = null)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Username == model.Username);

            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                return View(model);
            }

            var verified = Functions.VerifyPassword(user.PasswordHash, model.PasswordHash, out var needsUpgrade);
            if (!verified)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                return View(model);
            }

            if (!user.Status)
            {
                ViewBag.Error = "Tài khoản của bạn đã bị khóa hoặc chưa được kích hoạt!";
                return View(model);
            }

            if (needsUpgrade)
            {
                user.PasswordHash = Functions.HashPassword(model.PasswordHash);
                _context.Users.Update(user);
                _context.SaveChanges();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("UserCode", user.UserCode);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Email", user.Email ?? "");
            HttpContext.Session.SetString("PhoneNumber", user.PhoneNumber ?? "");
            HttpContext.Session.SetInt32("Role", user.Role);

            var roleName = user.Role == 1 ? "Admin" : "User";
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Role, roleName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            if (user.Role == 1)
            {
                return Redirect("/Admin");
            }

            if (IsSafeReturnUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
