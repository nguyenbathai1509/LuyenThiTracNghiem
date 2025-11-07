using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LuyenThiTracNghiem.Controllers
{
    public class RegisterController : Controller
    {
        private readonly DataContext _context;
        public RegisterController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(tblUser model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                return View(model);
            }

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                return View(model);
            }

            if (_context.Users.Any(u => u.PhoneNumber == model.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại đã tồn tại");
                return View(model);
            }

            var user = new tblUser
            {
                UserCode = Guid.NewGuid().ToString(),
                FullName = model.FullName,
                BirthDate = model.BirthDate,
                Gender = model.Gender,
                Username = model.Username,
                PasswordHash = model.PasswordHash,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Role = 2,
                Status = true,
                CreatedAt = DateTime.Now,
                CreatedBy = "user"
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("RegisterSuccess");
        }
        public IActionResult RegisterSuccess()
        {
            ViewBag.Message = "Đăng ký thành công! Vui lòng đăng nhập.";
            ViewBag.RedirectUrl = Url.Action("Index", "Login");
            ViewBag.Delay = 3;
            return View();
        }
    }
}