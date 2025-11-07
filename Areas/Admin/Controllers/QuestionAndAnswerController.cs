using System.ComponentModel.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Areas.Admin.Models;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using CsvHelper;
using System.Globalization;
using LuyenThiTracNghiem.Filters;

namespace LuyenThiTracNghiem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class QuestionAndAnswerController : Controller
    {
        private readonly DataContext _context;
        public QuestionAndAnswerController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 8)
        {
            ViewBag.Subjects = await _context.Subjects.ToListAsync();

            var query = _context.Questions.AsQueryable();

            int totalItems = await query.CountAsync();

            var questions = await query
                .OrderBy(s => s.QuestionId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new PagedResult<tblQuestion>
            {
                Items = questions,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        public async Task<IActionResult> LoadData(string? searchTerm, string? subjectId, QuestionLevel? level, int page = 1, int pageSize = 8)
        {
            var query = _context.Questions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(s => s.QuestionText.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(subjectId))
            {
                query = query.Where(s => s.SubjectId == subjectId);
            }

            if (level.HasValue)
            {
                query = query.Where(s => s.Level == level.Value);
            }

            int totalItems = await query.CountAsync();

            var questions = await query
                .Include(q => q.Subject)
                .OrderBy(s => s.QuestionId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new PagedResult<tblQuestion>
            {
                Items = questions,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return PartialView("_QuestionListPartial", model);
        }

        public async Task<IActionResult> Create()
        {
            var subjects = await _context.Subjects.ToListAsync();
            ViewBag.Subjects = new SelectList(subjects, "SubjectId", "SubjectName");
            return View(new QuestionCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuestionCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var subjects = await _context.Subjects.ToListAsync();
                ViewBag.Subjects = new SelectList(subjects, "SubjectId", "SubjectName", model.SubjectId);
                return View(model);
            }

            bool qsExists = _context.Questions.Any(q => q.QuestionText == model.QuestionText && q.SubjectId == model.SubjectId);

            if (qsExists)
            {
                ModelState.AddModelError("QuestionText", "Câu hỏi đã tồn tại trong mô học này! Vui lòng thêm câu hỏi khác");
                var subjects = await _context.Subjects.ToListAsync();
                ViewBag.Subjects = new SelectList(subjects, "SubjectId", "SubjectName", model.SubjectId);
                return View(model);
            }

            var question = new tblQuestion
            {
                SubjectId = model.SubjectId,
                QuestionText = model.QuestionText,
                Level = model.Level!.Value,
                Status = model.Status,
                CreatedAt = DateTime.Now,
                CreatedBy = "Admin"
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            for (int i = 0; i < model.Answers.Count; i++)
            {
                var answer = new tblAnswer
                {
                    QuestionId = question.QuestionId,
                    AnswerText = model.Answers[i].AnswerText,
                    IsCorrect = (i == model.CorrectAnswer),
                    Status = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "Admin"
                };
                _context.Answers.Add(answer);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm câu hỏi thành công!";
            return RedirectToAction("Create");
        }

        public async Task<IActionResult> Edit(int? questionId)
        {
            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);

            if (question == null)
            {
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            var model = new QuestionCreateViewModel
            {
                SubjectId = question.SubjectId,
                QuestionId = question.QuestionId,
                QuestionText = question.QuestionText,
                Level = question.Level,
                Status = question.Status,
                Answers = question.Answers.Select(a => new AnswerCreateModel
                {
                    AnswerText = a.AnswerText,
                    IsCorrect = a.IsCorrect
                }).ToList(),
                CorrectAnswer = question.Answers.ToList().FindIndex(a => a.IsCorrect)
            };

            var subjects = await _context.Subjects.ToListAsync();
            ViewBag.Subjects = new SelectList(subjects, "SubjectId", "SubjectName", model.SubjectId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(QuestionCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var subjects = await _context.Subjects.ToListAsync();
                ViewBag.Subjects = new SelectList(subjects, "SubjectId", "SubjectName", model.SubjectId);
                return View(model);
            }

            if (model.QuestionId == null || model.SubjectId == null)
            {
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            var question = await _context.Questions.Include(q => q.Answers).FirstOrDefaultAsync(q => q.QuestionId == model.QuestionId);

            if (question == null)
            {
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            bool checkQuestionText = await _context.Questions.AnyAsync(c => c.QuestionText == model.QuestionText && c.QuestionId != model.QuestionId && c.SubjectId == model.SubjectId);

            if (checkQuestionText)
            {
                var subjects = await _context.Subjects.ToListAsync();
                ViewBag.Subjects = new SelectList(subjects, "SubjectId", "SubjectName", model.SubjectId);
                ModelState.AddModelError("QuestionText", "Câu hỏi đã tồn tại trong môn học này !");
                return View(model);
            }

            question.QuestionText = model.QuestionText;
            question.Level = model.Level!.Value;
            question.Status = model.Status;
            question.UpdatedAt = DateTime.Now;
            question.UpdatedBy = "admin";

            var questionAnswers = question.Answers.ToList();

            for (int i = 0; i < model.Answers.Count; i++)
            {
                if (i < questionAnswers.Count)
                {
                    questionAnswers[i].AnswerText = model.Answers[i].AnswerText;
                    questionAnswers[i].IsCorrect = i == model.CorrectAnswer;
                    questionAnswers[i].UpdatedAt = DateTime.Now;
                    questionAnswers[i].UpdatedBy = "admin";
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật câu hỏi và đáp án thành công!";
            return RedirectToAction("Edit", new { QuestionId = model.QuestionId });
        }

        public IActionResult Delete(int? questionId)
        {
            if (questionId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy câu hỏi!";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            var question = _context.Questions.FirstOrDefault(q => q.QuestionId == questionId);
            if (question == null)
            {
                TempData["ErrorMessage"] = "Câu hỏi không tồn tại!";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed([FromBody] DeleteQuestionRequest request)
        {
            if (request == null || request.Id <= 0)
                return BadRequest();

            var question = _context.Questions.FirstOrDefault(q => q.QuestionId == request.Id);
            if (question == null)
                return NotFound();

            var answers = _context.Answers.Where(a => a.QuestionId == question.QuestionId).ToList();
            if (answers.Any())
                _context.Answers.RemoveRange(answers);

            var questionInExams = _context.QuestionInExams.Where(qe => qe.QuestionId == question.QuestionId).ToList();
            if(questionInExams.Any())
                _context.QuestionInExams.RemoveRange(questionInExams);

            _context.Questions.Remove(question);
            _context.SaveChanges();

            return Ok(new { message = "Xóa câu hỏi và các đáp án thành công!" });
        }
    }

    public class DeleteQuestionRequest
    {
        public int Id { get; set; }
    }
}