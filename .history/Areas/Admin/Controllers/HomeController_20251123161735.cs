using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Areas.Admin.Models;
using LuyenThiTracNghiem.Filters;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuyenThiTracNghiem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class HomeController : Controller
    {
        private readonly DataContext _context;

        public HomeController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var model = new AdminStatsViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalSubjects = await _context.Subjects.CountAsync(),
                TotalExams = await _context.Exams.CountAsync(),
                TotalCompletedAttempts = await _context.ExamAttempts.CountAsync(a => a.IsCompleted),
                TopExams = await _context.ExamAttempts
                    .Where(a => a.IsCompleted && a.Exam != null)
                    .GroupBy(a => new { a.ExamId, a.Exam!.ExamName, SubjectName = a.Exam.Subject != null ? a.Exam.Subject.SubjectName : string.Empty })
                    .Select(g => new AdminTopExamStatsViewModel
                    {
                        ExamName = g.Key.ExamName,
                        SubjectName = g.Key.SubjectName ?? string.Empty,
                        AttemptCount = g.Count(),
                        AvgScore = g.Any() ? g.Average(x => x.Score ?? 0) : 0
                    })
                    .OrderByDescending(x => x.AttemptCount)
                    .ThenByDescending(x => x.AvgScore)
                    .Take(5)
                    .ToListAsync(),
                TopDepositUsers = await _context.Payments
                    .Where(p => p.PaymentStatus == "Success")
                    .GroupBy(p => new
                    {
                        p.UserId,
                        UserName = p.User != null && !string.IsNullOrWhiteSpace(p.User.FullName)
                            ? p.User.FullName
                            : p.User != null ? p.User.Username : "Không xác định"
                    })
                    .Select(g => new AdminTopUserDepositViewModel
                    {
                        UserId = g.Key.UserId,
                        UserName = g.Key.UserName,
                        TotalAmount = g.Sum(x => x.Amount),
                        DepositCount = g.Count()
                    })
                    .OrderByDescending(x => x.TotalAmount)
                    .ThenByDescending(x => x.DepositCount)
                    .Take(5)
                    .ToListAsync()
            };

            return View(model);
        }
    }
}