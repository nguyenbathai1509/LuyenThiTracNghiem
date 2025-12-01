using System.ComponentModel.Design;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using LuyenThiTracNghiem.Areas.Admin.Models;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

        [HttpGet]
        public IActionResult UploadQuestions()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadQuestions(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file CSV/XLSX để tải lên.";
                return View();
            }

            var ext = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".csv" && ext != ".xlsx")
            {
                TempData["ErrorMessage"] = "Chỉ hỗ trợ file .csv hoặc .xlsx.";
                return View();
            }

            int created = 0;
            int skipped = 0;

            if (ext == ".csv")
            {
                using var reader = new StreamReader(file.OpenReadStream());
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                var rows = csv.GetRecords<ImportQuestionRow>().ToList();
                ImportRows(rows, ref created, ref skipped);
            }
            else if (ext == ".xlsx")
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(file.OpenReadStream());
                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws != null)
                {
                    var rows = new List<ImportQuestionRow>();
                    var startRow = 2; // row 1 header
                    var lastRow = ws.Dimension?.End.Row ?? 0;
                    for (int r = startRow; r <= lastRow; r++)
                    {
                        rows.Add(new ImportQuestionRow
                        {
                            SubjectId = ws.Cells[r, 1].Text?.Trim(),
                            QuestionText = ws.Cells[r, 2].Text?.Trim(),
                            Level = ws.Cells[r, 3].Text?.Trim(),
                            Answer1 = ws.Cells[r, 4].Text?.Trim(),
                            Answer2 = ws.Cells[r, 5].Text?.Trim(),
                            Answer3 = ws.Cells[r, 6].Text?.Trim(),
                            Answer4 = ws.Cells[r, 7].Text?.Trim(),
                            Correct = ws.Cells[r, 8].Text?.Trim()
                        });
                    }
                    ImportRows(rows, ref created, ref skipped);
                }
            }

            TempData["SuccessMessage"] = $"Đã import {created} câu hỏi. Bỏ qua {skipped} dòng không hợp lệ hoặc trùng.";
            return View();
        }

        private void ImportRows(IEnumerable<ImportQuestionRow> rows, ref int created, ref int skipped)
        {
            foreach (var row in rows)
            {
                if (row == null) { skipped++; continue; }
                if (string.IsNullOrWhiteSpace(row.SubjectId) || string.IsNullOrWhiteSpace(row.QuestionText))
                {
                    skipped++; continue;
                }

                var subjectId = row.SubjectId.Trim();
                var subjectExists = _context.Subjects.Any(s => s.SubjectId == subjectId);
                if (!subjectExists)
                {
                    skipped++; continue;
                }

                if (!Enum.TryParse<QuestionLevel>(row.Level, out var level))
                {
                    // allow numeric level
                    if (byte.TryParse(row.Level, out var levelNum) && Enum.IsDefined(typeof(QuestionLevel), (QuestionLevel)levelNum))
                    {
                        level = (QuestionLevel)levelNum;
                    }
                    else
                    {
                        skipped++; continue;
                    }
                }

                var answers = new[] { row.Answer1, row.Answer2, row.Answer3, row.Answer4 }
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .ToList();
                if (!answers.Any())
                {
                    skipped++; continue;
                }

                int correctIndex = ParseCorrectIndex(row.Correct);
                if (correctIndex < 0 || correctIndex >= answers.Count)
                {
                    skipped++; continue;
                }

                // tránh trùng câu hỏi trong cùng môn
                bool exists = _context.Questions.Any(q => q.QuestionText == row.QuestionText && q.SubjectId == subjectId);
                if (exists)
                {
                    skipped++; continue;
                }

                var question = new tblQuestion
                {
                    SubjectId = subjectId,
                    QuestionText = row.QuestionText,
                    Level = level,
                    Status = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "Admin import"
                };
                _context.Questions.Add(question);
                _context.SaveChanges();

                for (int i = 0; i < answers.Count; i++)
                {
                    var answer = new tblAnswer
                    {
                        QuestionId = question.QuestionId,
                        AnswerText = answers[i]!,
                        IsCorrect = (i == correctIndex),
                        Status = true,
                        CreatedAt = DateTime.Now,
                        CreatedBy = "Admin import"
                    };
                    _context.Answers.Add(answer);
                }
                _context.SaveChanges();
                created++;
            }
        }

        private int ParseCorrectIndex(string? correct)
        {
            if (string.IsNullOrWhiteSpace(correct)) return -1;
            correct = correct.Trim();
            // Accept 0-based, 1-based, or A/B/C/D
            if (int.TryParse(correct, out var num))
            {
                return num > 0 ? num - 1 : num;
            }
            var upper = correct.ToUpperInvariant();
            return upper switch
            {
                "A" => 0,
                "B" => 1,
                "C" => 2,
                "D" => 3,
                _ => -1
            };
        }

        private class ImportQuestionRow
        {
            public string? SubjectId { get; set; }
            public string? QuestionText { get; set; }
            public string? Level { get; set; }
            public string? Answer1 { get; set; }
            public string? Answer2 { get; set; }
            public string? Answer3 { get; set; }
            public string? Answer4 { get; set; }
            public string? Correct { get; set; }
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
