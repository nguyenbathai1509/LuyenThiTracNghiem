using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuyenThiTracNghiem.Controllers
{
    public class ChangePasswordController : Controller
    {
        private readonly DataContext _context;
        public ChangePasswordController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string OldPassword, string NewPassword, string ConfirmPassword)
        {

            int? userID = HttpContext.Session.GetInt32("UserId");

            if (userID == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = _context.Users.Where(u => u.UserId == userID).FirstOrDefault();

            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (user.PasswordHash != OldPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu cũ không đúng.";
                return RedirectToAction("Index");
            }

            if (NewPassword != ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu xác nhận không khớp.";
                return RedirectToAction("Index");
            }

            user.PasswordHash = NewPassword;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = "user";

            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Index");
        }
    }
}