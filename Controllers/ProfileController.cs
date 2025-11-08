using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuyenThiTracNghiem.Controllers
{
    public class ProfileController : Controller
    {
        private readonly DataContext _context;
        public ProfileController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
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
            return View("Index", user);
        }

        [HttpPost]
        public IActionResult Index(tblUser model)
        {
            int? userID = HttpContext.Session.GetInt32("UserId");
            if (userID == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userID);
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            bool checkPhone = _context.Users.Any(u => u.UserId != userID && u.PhoneNumber == model.PhoneNumber);
            if (checkPhone)
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại đã tồn tại !");
                return View(model);
            }

            bool checkEmail = _context.Users.Any(u => u.UserId != userID && u.Email == model.Email);
            if (checkEmail)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại !");
                return View(model);
            }

            user.FullName = model.FullName;
            user.BirthDate = model.BirthDate;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = "user";

            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAccount([FromBody] DeleteAccountRequest request)
        {
            if (request == null || request.Id <= 0)
                return BadRequest();

            int? sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null || sessionUserId != request.Id)
            {
                return Unauthorized();
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == request.Id);
            if (user == null)
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });

            _context.Users.Remove(user);
            _context.SaveChanges();

            HttpContext.Session.Clear();

            return Ok();
        }
    }

    public class DeleteAccountRequest
    {
        public int Id { get; set; }
    }
}