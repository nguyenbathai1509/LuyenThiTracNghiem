using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LuyenThiTracNghiem.Areas.Admin.Models;
using LuyenThiTracNghiem.Models;
using LuyenThiTracNghiem.Models.ViewModels;

namespace LuyenThiTracNghiem.Controllers
{
    public class TakeExamController : Controller
    {
        private readonly DataContext _context;
        public TakeExamController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Start(int? id)
        {
            // kiểm tra login
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để làm bài.";
                return Redirect("/Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId.Value);
            if (user == null) return NotFound();

            if (id == null) return NotFound();

            var exam = _context.Exams.FirstOrDefault(e => e.ExamId == id.Value && e.Status == true);
            if (exam == null) return NotFound();

            if (user.Balance < exam.ExamFee)
            {
                TempData["ErrorMessage"] = "Số dư trong ví không đủ để làm bài thi này !";
                return RedirectToAction("ExamInfor", "Home", new { id = exam.ExamId });
            }

            // Tạo attempt + trừ tiền trong transaction đơn giản
            using (var tx = _context.Database.BeginTransaction())
            {
                try
                {
                    if (exam.ExamFee > 0)
                    {
                        user.Balance -= exam.ExamFee;
                        _context.Users.Update(user);
                    }

                    var attempt = new tblExamAttempt
                    {
                        ExamId = exam.ExamId,
                        UserId = user.UserId,
                        StartedAt = DateTime.UtcNow,
                        TimeLimitSeconds = exam.DurationMinutes * 60,
                        IsCompleted = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = string.IsNullOrWhiteSpace(user.FullName) ? user.Username : user.FullName
                    };

                    _context.ExamAttempts.Add(attempt);
                    _context.SaveChanges();

                    tx.Commit();

                    return RedirectToAction("Take", new { attemptId = attempt.AttemptId });
                }
                catch
                {
                    tx.Rollback();
                    TempData["ErrorMessage"] = "Không thể bắt đầu làm bài. Vui lòng thử lại.";
                    return RedirectToAction("ExamInfor", "Home", new { id = exam.ExamId });
                }
            }
        }

        [HttpGet]
        public IActionResult Take(int attemptId)
        {
            // load attempt
            var attempt = _context.ExamAttempts.FirstOrDefault(a => a.AttemptId == attemptId);
            if (attempt == null) return NotFound();

            // kiểm tra quyền
            var userId = HttpContext.Session.GetInt32("UserId");
            if (attempt.UserId != null && attempt.UserId != userId)
            {
                return Forbid();
            }

            var exam = _context.Exams.FirstOrDefault(e => e.ExamId == attempt.ExamId);
            if (exam == null) return NotFound();

            // time remaining (server-side)
            var elapsed = (int)(DateTime.UtcNow - attempt.StartedAt).TotalSeconds;
            var timeLimit = attempt.TimeLimitSeconds ?? (exam.DurationMinutes * 60);
            var remaining = Math.Max(0, timeLimit - elapsed);

            // get question list from QuestionInExam (ordered by CreatedAt)
            var qInExam = _context.QuestionInExams
                                .Where(q => q.ExamId == exam.ExamId)
                                .OrderBy(q => q.CreatedAt)
                                .ToList();

            if (!qInExam.Any())
            {
                TempData["ErrorMessage"] = "Đề thi chưa có câu hỏi.";
                return RedirectToAction("ExamInfor", "Home", new { id = exam.ExamId });
            }

            var questionIds = qInExam.Select(q => q.QuestionId).ToList();

            var questions = _context.Questions
                            .Where(q => questionIds.Contains(q.QuestionId) && q.Status)
                            .ToList();

            var answers = _context.Answers
                            .Where(a => questionIds.Contains(a.QuestionId) && a.Status)
                            .ToList();

            var questionDtos = qInExam
                .Where(qie => questions.Any(q => q.QuestionId == qie.QuestionId))
                .Select(qie =>
                {
                    var q = questions.First(x => x.QuestionId == qie.QuestionId);
                    return new QuestionDto
                    {
                        QuestionId = q.QuestionId,
                        QuestionText = q.QuestionText,
                        Answers = answers
                            .Where(a => a.QuestionId == q.QuestionId)
                            .Select(a => new AnswerDto { AnswerId = a.AnswerId, AnswerText = a.AnswerText })
                            .ToList()
                    };
                })
                .ToList();

            var model = new TakeExamViewModel
            {
                AttemptId = attempt.AttemptId,
                ExamId = exam.ExamId,
                ExamName = exam.ExamName,
                TimeRemainingSeconds = remaining,
                Questions = questionDtos
            };

            // View file: Views/TakeExam/Take.cshtml
            return View("Index", model);
        }
    }
}
