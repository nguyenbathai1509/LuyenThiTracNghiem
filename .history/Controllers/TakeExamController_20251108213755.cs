using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LuyenThiTracNghiem.Controllers
{
    public class TakeExamController : Controller
    {
        private readonly DataContext _context;
        public TakeExamController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Start(int? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var user = _context.Users.Where(u => u.UserId == userId).FirstOrDefault();

            if (user == null)
                return NotFound();

            if (id == null)
                return NotFound();

            var exam = _context.Exams.Where(e => e.ExamId == id && e.Status == true).FirstOrDefault();

            if (exam == null)
                return NotFound();

            if (user.Balance < exam.ExamFee)
            {
                TempData["ErrorMessage"] = "Số dư trong ví không đủ để làm bài thi này !";
                return RedirectToAction("ExamInfor", "Home", new { id = exam.ExamId });
            }

            if (exam.ExamFee > 0)
            {
                user.Balance -= exam.ExamFee;
                _context.Users.Update(user);
                _context.SaveChanges();
            }

            var attempt = new ExamAttempt
            {
                ExamId = exam.ExamId,
                UserId = user.UserId,
                StartedAt = DateTime.UtcNow,
                TimeLimitSeconds = exam.DurationMinutes * 60,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.FullName ?? user.Username
            };

        }
    }
}