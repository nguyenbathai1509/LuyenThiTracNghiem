using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Areas.Admin.Models;
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

            var attempt = new tblExamAttempt
            {
                ExamId = exam.ExamId,
                UserId = user.UserId,
                StartedAt = DateTime.UtcNow,
                TimeLimitSeconds = exam.DurationMinutes * 60,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.FullName ?? user.Username
            };

            _context.ExamAttempts.Add(attempt);
            _context.SaveChanges();

            return RedirectToAction("Take", "TakeExam", new { attemptId = attempt.AttemptId });

        }

        public IActionResult Take(int attemptId)
        {
            // load attempt + exam
            var attempt = _context.ExamAttempts
                .FirstOrDefault(a => a.AttemptId == attemptId);

            if (attempt == null) return NotFound();

            // kiểm tra quyền: user phải là chủ attempt
            var userId = HttpContext.Session.GetInt32("UserId");
            if (attempt.UserId != null && attempt.UserId != userId)
            {
                return Forbid();
            }

            var exam = _context.Exams.FirstOrDefault(e => e.ExamId == attempt.ExamId);
            if (exam == null) return NotFound();

            // Tính thời gian còn lại (server là nguồn chính xác)
            var elapsed = (int)(DateTime.UtcNow - attempt.StartedAt).TotalSeconds;
            var timeLimit = attempt.TimeLimitSeconds ?? (exam.DurationMinutes * 60);
            var remaining = Math.Max(0, timeLimit - elapsed);

            // Lấy bảng tblQuestionInExam (theo thứ tự). 
            // Nếu bạn có trường order/sort hãy dùng nó (ví dụ .OrderBy(q => q.SortOrder)).
            var qInExam = _context.QuestionInExams
                                .Where(q => q.ExamId == exam.ExamId)
                                .OrderBy(q => q.CreatedAt) // hoặc .OrderBy(q => q.YourOrderField)
                                .ToList();

            var questionIds = qInExam.Select(q => q.QuestionId).ToList();

            // Lấy chi tiết câu hỏi
            var questions = _context.Questions
                            .Where(q => questionIds.Contains(q.QuestionId) && q.Status)
                            .ToList();

            // Lấy tất cả đáp án (choices) cho những câu trên
            var answers = _context.Answers
                            .Where(a => questionIds.Contains(a.QuestionId) && a.Status)
                            .ToList();

            // Build DTO theo thứ tự của qInExam
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

            return View("Home""TakeExam", model); // Views/TakeExam/Take.cshtml
        }

        public class TakeExamViewModel
        {
            public int AttemptId { get; set; }
            public int ExamId { get; set; }
            public string ExamName { get; set; } = string.Empty;
            public int TimeRemainingSeconds { get; set; }
            public List<QuestionDto> Questions { get; set; } = new();
        }

        public class QuestionDto
        {
            public int QuestionId { get; set; }
            public string QuestionText { get; set; } = string.Empty;
            public List<AnswerDto> Answers { get; set; } = new();
        }

        public class AnswerDto
        {
            public int AnswerId { get; set; }
            public string AnswerText { get; set; } = string.Empty;
        }


    }
}