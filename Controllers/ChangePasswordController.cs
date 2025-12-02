using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LuyenThiTracNghiem.Utilities;

namespace LuyenThiTracNghiem.Controllers
{
    [Authorize]
    public class ChangePasswordController : Controller
    {
        private readonly DataContext _context;
        public ChangePasswordController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userID = HttpContext.Session.GetInt32("UserId");
            if (userID == null)
            {
                var returnUrl = Request.Path + Request.QueryString;
                return RedirectToAction("Index", "Login", new { returnUrl });
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(string OldPassword, string NewPassword, string ConfirmPassword)
        {

            int? userID = HttpContext.Session.GetInt32("UserId");

            if (userID == null)
            {
                var returnUrl = Request.Path + Request.QueryString;
                return RedirectToAction("Index", "Login", new { returnUrl });
            }

            var user = _context.Users.Where(u => u.UserId == userID).FirstOrDefault();

            if (user == null)
            {
                var returnUrl = Request.Path + Request.QueryString;
                return RedirectToAction("Index", "Login", new { returnUrl });
            }

            var oldHash = Functions.MD5Password(OldPassword);
            if (user.PasswordHash != oldHash)
            {
                TempData["ErrorMessage"] = "Mật khẩu cũ không đúng.";
                return RedirectToAction("Index");
            }

            if (NewPassword != ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu xác nhận không khớp.";
                return RedirectToAction("Index");
            }

            user.PasswordHash = Functions.MD5Password(NewPassword);
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = "user";

            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Index");
        }
    }
}
