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

        [HttpGet("/Login")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(tblUser model)
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
            
            return RedirectToAction("Index", "Home");
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}