using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LuyenThiTracNghiem.Areas.Admin.Models;
using LuyenThiTracNghiem.Models;
using LuyenThiTracNghiem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

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
                return RedirectToRoute("ExamInfor", new { slug = SlugGenerator.SlugGenerator.GenerateSlug(exam.ExamName), id = exam.ExamId });
            }

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
                    return RedirectToRoute("ExamInfor", new { slug = SlugGenerator.SlugGenerator.GenerateSlug(exam.ExamName), id = exam.ExamId });
                }
            }
        }

        [HttpGet]
        public IActionResult Take(int attemptId)
        {
            var attempt = _context.ExamAttempts.FirstOrDefault(a => a.AttemptId == attemptId);
            if (attempt == null) return NotFound();

            var userId = HttpContext.Session.GetInt32("UserId");
            if (attempt.UserId != null && attempt.UserId != userId)
            {
                return Forbid();
            }

            var exam = _context.Exams.FirstOrDefault(e => e.ExamId == attempt.ExamId);
            if (exam == null) return NotFound();

            var elapsed = (int)(DateTime.UtcNow - attempt.StartedAt).TotalSeconds;
            var timeLimit = attempt.TimeLimitSeconds ?? (exam.DurationMinutes * 60);
            var remaining = Math.Max(0, timeLimit - elapsed);

            var qInExam = _context.QuestionInExams
                                .Where(q => q.ExamId == exam.ExamId)
                                .OrderBy(q => q.CreatedAt)
                                .ToList();

            if (!qInExam.Any())
            {
                TempData["ErrorMessage"] = "Đề thi chưa có câu hỏi.";
                return RedirectToRoute("ExamInfor", new { slug = SlugGenerator.SlugGenerator.GenerateSlug(exam.ExamName), id = exam.ExamId });
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

            return View("Index", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(SubmitExamModel model)
        {
            if (model == null || model.AttemptId <= 0) return BadRequest();

            var attempt = await _context.ExamAttempts
                .Include(a => a.Exam)
                .FirstOrDefaultAsync(a => a.AttemptId == model.AttemptId);

            if (attempt == null) return NotFound();
            if (attempt.IsCompleted) return BadRequest("Bài thi đã nộp.");

            int correct = 0, wrong = 0, unanswered = 0;

            foreach (var ans in model.Answers)
            {
                var correctAnswer = await _context.Answers
                    .Where(a => a.QuestionId == ans.QuestionId && a.IsCorrect)
                    .FirstOrDefaultAsync();

                bool isCorrect = (correctAnswer != null && correctAnswer.AnswerId == ans.SelectedAnswerId);

                if (!ans.SelectedAnswerId.HasValue)
                    unanswered++;
                else if (isCorrect)
                    correct++;
                else
                    wrong++;

                var attemptAnswer = new tblAttemptAnswer
                {
                    AttemptId = attempt.AttemptId,
                    QuestionId = ans.QuestionId,
                    AnswerId = ans.SelectedAnswerId,
                    IsCorrect = isCorrect,
                    CreatedAt = DateTime.UtcNow
                };
                _context.AttemptAnswers.Add(attemptAnswer);
            }

            int total = correct + wrong + unanswered;
            attempt.CorrectCount = correct;
            attempt.WrongCount = wrong;
            attempt.UnansweredCount = unanswered;
            attempt.Score = total > 0 ? (decimal)correct * 10 / total : 0;
            attempt.PercentScore = total > 0 ? (decimal)correct * 100 / total : 0;
            attempt.IsCompleted = true;
            attempt.FinishedAt = DateTime.UtcNow;
            attempt.DurationSeconds = (int)(DateTime.UtcNow - attempt.StartedAt).TotalSeconds;

            await _context.SaveChangesAsync();

            return RedirectToAction("Result", new { attemptId = attempt.AttemptId });
        }

        // GET: hiển thị kết quả
        public async Task<IActionResult> Result(int attemptId)
        {
            var attempt = await _context.ExamAttempts
                .Include(a => a.Exam)
                .FirstOrDefaultAsync(a => a.AttemptId == attemptId);

            if (attempt == null) return NotFound();

            var questions = await _context.Questions
                .Where(q => q.SubjectId == attempt.Exam!.SubjectId) // dùng SubjectId
                .Select(q => new ResultQuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    CorrectAnswerId = q.Answers.FirstOrDefault(a => a.IsCorrect)!.AnswerId,
                    Answers = q.Answers.Select(a => new AnswerDto
                    {
                        AnswerId = a.AnswerId,
                        AnswerText = a.AnswerText
                    }).ToList(),
                    UserAnswerId = _context.AttemptAnswers
                                .Where(aa => aa.AttemptId == attempt.AttemptId && aa.QuestionId == q.QuestionId)
                                .Select(aa => (int?)aa.AnswerId)
                                .FirstOrDefault()
                }).ToListAsync();

            var vm = new ExamResultViewModel
            {
                ExamName = attempt.Exam.ExamName,
                Score = attempt.Score ?? 0,
                CorrectCount = attempt.CorrectCount ?? 0,
                IncorrectCount = attempt.WrongCount ?? 0,
                UnansweredCount = attempt.UnansweredCount ?? 0,
                Questions = questions
            };

            return View(vm);
        }
    }
}
