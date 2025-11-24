using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuyenThiTracNghiem.Models;
using LuyenThiTracNghiem.Models.ViewModels;
using LuyenThiTracNghiem.Areas.Admin.Models;

namespace LuyenThiTracNghiem.Controllers
{
    public class UserStatsController : Controller
    {
        private readonly DataContext _context;

        public UserStatsController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null) return NotFound();

            var attempts = _context.ExamAttempts
                .Include(a => a.Exam)
                    .ThenInclude(e => e.Subject)
                .Where(a => a.UserId == id && a.IsCompleted)
                .ToList();

            var totalAttempts = attempts.Count;
            var totalExamsTaken = attempts.Select(a => a.ExamId).Distinct().Count();
            var avgScore = attempts.Any() ? attempts.Average(a => a.Score ?? 0) : 0;
            var avgPercent = attempts.Any() ? attempts.Average(a => a.PercentScore ?? 0) : 0;
            var totalCorrect = attempts.Sum(a => a.CorrectCount ?? 0);
            var totalWrong = attempts.Sum(a => a.WrongCount ?? 0);
            var totalUnanswered = attempts.Sum(a => a.UnansweredCount ?? 0);

            var payments = _context.Payments
                .Where(p => p.UserId == id)
                .OrderByDescending(p => p.PaymentDate )
                .ToList();

            var successfulPayments = payments.Where(p => p.PaymentStatus == "Success").ToList();
            var totalDeposits = payments.Count;
            var successfulDeposits = successfulPayments.Count;
            var pendingDeposits = payments.Count(p => p.PaymentStatus == "Pending");
            var failedDeposits = payments.Count(p => p.PaymentStatus == "Failed");
            var totalDepositAmount = successfulPayments.Sum(p => p.Amount);
            var lastDeposit = payments.FirstOrDefault();

            var subjectStats = attempts
                .Where(a => a.Exam != null && a.Exam.Subject != null)
                .GroupBy(a => new { a.Exam!.SubjectId, a.Exam.Subject!.SubjectName })
                .Select(g => new UserSubjectStatsViewModel
                {
                    SubjectName = g.Key.SubjectName ?? string.Empty,
                    ExamsTaken = g.Select(x => x.ExamId).Distinct().Count(),
                    AvgPercentScore = g.Any() ? g.Average(x => x.PercentScore ?? 0) : 0,
                    TotalCorrect = g.Sum(x => x.CorrectCount ?? 0),
                    TotalWrong = g.Sum(x => x.WrongCount ?? 0)
                })
                .OrderByDescending(s => s.AvgPercentScore)
                .ToList();

            var attemptIds = attempts.Select(a => a.AttemptId).ToList();

            var difficultyStats = attemptIds.Any()
                ? _context.AttemptAnswers
                    .Include(aa => aa.Question)
                    .Where(aa => attemptIds.Contains(aa.AttemptId) && aa.Question != null)
                    .GroupBy(aa => aa.Question!.Level)
                    .Select(g => new UserDifficultyStatsViewModel
                    {
                        Level = g.Key,
                        TotalAnswered = g.Count(),
                        CorrectCount = g.Count(x => x.IsCorrect),
                        AccuracyPercent = g.Count() > 0 ? (decimal)g.Count(x => x.IsCorrect) * 100 / g.Count() : 0
                    })
                    .OrderBy(ds => ds.Level)
                    .ToList()
                : new System.Collections.Generic.List<UserDifficultyStatsViewModel>();

            var model = new UserStatsViewModel
            {
                UserId = user.UserId,
                FullName = string.IsNullOrWhiteSpace(user.FullName) ? user.Username : user.FullName,
                TotalAttempts = totalAttempts,
                TotalExamsTaken = totalExamsTaken,
                AvgScore = avgScore,
                AvgPercentScore = avgPercent,
                TotalCorrect = totalCorrect,
                TotalWrong = totalWrong,
                TotalUnanswered = totalUnanswered,
                TotalDeposits = totalDeposits,
                SuccessfulDeposits = successfulDeposits,
                PendingDeposits = pendingDeposits,
                FailedDeposits = failedDeposits,
                TotalDepositAmount = totalDepositAmount,
                LastDepositDate = lastDeposit?.PaymentDate,
                LastDepositAmount = lastDeposit?.Amount,
                SubjectStats = subjectStats,
                DifficultyStats = difficultyStats
            };

            return View(model);
        }
    }
}
