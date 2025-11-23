using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuyenThiTracNghiem.Models;
using LuyenThiTracNghiem.Models.ViewModels;

namespace LuyenThiTracNghiem.Controllers
{
    public class ExamHistoryController : Controller
    {
        private readonly DataContext _context;

        public ExamHistoryController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();

            var attempts = _context.ExamAttempts
                .Include(a => a.Exam)
                    .ThenInclude(e => e.Subject)
                .Where(a => a.UserId == id && a.IsCompleted)
                .OrderByDescending(a => a.FinishedAt ?? a.StartedAt)
                .ToList();

            var attemptItems = attempts.Select(a =>
            {
                var attemptTime = a.FinishedAt ?? a.StartedAt;
                TimeSpan? duration = null;
                if (a.FinishedAt.HasValue)
                {
                    duration = a.FinishedAt.Value - a.StartedAt;
                }
                else if (a.DurationSeconds.HasValue)
                {
                    duration = TimeSpan.FromSeconds(a.DurationSeconds.Value);
                }

                var percent = a.PercentScore ?? (a.Score.HasValue ? a.Score * 10 : null);
                var isPassed = (percent ?? 0) >= 50;

                return new ExamHistoryItemViewModel
                {
                    AttemptId = a.AttemptId,
                    ExamName = a.Exam?.ExamName ?? "N/A",
                    SubjectName = a.Exam?.Subject?.SubjectName ?? "N/A",
                    AttemptTime = attemptTime,
                    Score = a.Score,
                    PercentScore = a.PercentScore,
                    Duration = duration,
                    IsPassed = isPassed
                };
            }).ToList();

            var model = new ExamHistoryViewModel
            {
                UserId = user.UserId,
                UserName = string.IsNullOrWhiteSpace(user.FullName) ? user.Username : user.FullName,
                Attempts = attemptItems
            };

            return View(model);
        }
    }
}
