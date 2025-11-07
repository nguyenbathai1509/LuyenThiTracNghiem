using System.Net.Mime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Areas.Admin.Models;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LuyenThiTracNghiem.Filters;

namespace LuyenThiTracNghiem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnly]
    public class ExamController : Controller
    {
        private readonly DataContext _context;

        public ExamController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 8)
        {
            ViewBag.Subjects = await _context.Subjects.ToListAsync();

            var query = _context.Exams.AsQueryable();
            int totalItems = await query.CountAsync();

            var exams = await query
                .OrderBy(s => s.ExamId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new PagedResult<tblExam>
            {
                Items = exams,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };

            return View(viewModel);
        }


        public async Task<IActionResult> LoadData(string searchTerm, string subjectId, string examType, int page = 1, int pageSize = 8)
        {
            var query = _context.Exams.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => s.ExamName.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(subjectId))
            {
                query = query.Where(s => s.SubjectId == subjectId);
            }

            if (!string.IsNullOrEmpty(examType))
            {
                query = query.Where(s => s.ExamType == examType);
            }

            int totalItems = await query.CountAsync();

            var exams = await query
                .OrderBy(s => s.ExamId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new PagedResult<tblExam>
            {
                Items = exams,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return PartialView("_ExamListPartial", model);
        }

        private void LoadSubjects()
        {
            var sjList = _context.Subjects
                .Select(s => new SelectListItem
                {
                    Value = s.SubjectId.ToString(),
                    Text = s.SubjectName
                }).ToList();

            sjList.Insert(0, new SelectListItem
            {
                Text = "--- Subject Name ---",
                Value = "0"
            });

            ViewBag.sjList = sjList;
        }


        public IActionResult Create()
        {
            LoadSubjects();
            return View();
        }

        [HttpPost]
        public IActionResult Create(tblExam exam, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (exam.SubjectId == "0")
                {
                    TempData["ErrorMessage"] = "Bạn phải chọn một môn học!";
                    LoadSubjects();
                    return View(exam);
                }

                bool exNameExists = _context.Exams.Any(n => n.ExamName == exam.ExamName && n.SubjectId == exam.SubjectId);
                if (exNameExists)
                {
                    ModelState.AddModelError("ExamName", "Tên đề thi đã tồn tại !");
                    LoadSubjects();
                    return View(exam);
                }

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/images/img-exam", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }
                    exam.Image = "/admin/images/img-exam/" + fileName;
                }

                exam.CreatedAt = DateTime.Now;
                exam.CreatedBy = "admin";
                _context.Exams.Add(exam);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Thêm đề thi thành công!";
                return RedirectToAction("Create");
            }

            LoadSubjects();
            return View(exam);
        }

        public IActionResult Edit(int? examId)
        {
            if (examId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đề thi !";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            var exam = _context.Exams.Find(examId);

            if (exam == null)
            {
                TempData["ErrorMessage"] = "Đề thi không tồn tại !";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            LoadSubjects();

            return View(exam);
        }

        [HttpPost]
        public IActionResult Edit(tblExam exam, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                var oldExam = _context.Exams.FirstOrDefault(e => e.ExamId == exam.ExamId);
                if (oldExam == null)
                {
                    TempData["ErrorMessage"] = "Đề thi không tồn tại !";
                    return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
                }

                bool exNameExists = _context.Exams.Any(e => e.ExamName == exam.ExamName && e.SubjectId == exam.SubjectId && e.ExamId != exam.ExamId);
                if (exNameExists)
                {
                    ModelState.AddModelError("ExamName", "Tên đề thi đã tồn tại !");
                    LoadSubjects();
                    return View(exam);
                }

                oldExam.ExamName = exam.ExamName;
                oldExam.Description = exam.Description;
                oldExam.QuestionCount = exam.QuestionCount;
                oldExam.DurationMinutes = exam.DurationMinutes;
                oldExam.ExamType = exam.ExamType;
                oldExam.ExamFee = exam.ExamFee;
                oldExam.Status = exam.Status;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(oldExam.Image))
                    {
                        var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/images/img-exam", oldExam.Image);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/images/img-exam", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }
                    oldExam.Image = "/admin/images/img-exam/" + fileName;
                }

                oldExam.UpdatedAt = DateTime.Now;
                oldExam.UpdatedBy = "admin";

                _context.SaveChanges();

                TempData["SuccessMessage"] = "Cập nhật thông tin đề thi thành công !";
                return RedirectToAction("Edit", new { ExamId = exam.ExamId });
            }

            LoadSubjects();
            return View(exam);
        }

        public async Task<IActionResult> EditQuestionInExam(int examId, string? searchTerm, QuestionLevel? filterLevel)
        {
            var exam = await _context.Exams.FindAsync(examId);
            if (exam == null)
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });

            var query = _context.Questions.Where(q => q.SubjectId == exam.SubjectId);

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(q => q.QuestionText.Contains(searchTerm));

            if (filterLevel.HasValue)
                query = query.Where(q => q.Level == filterLevel.Value);

            var questions = await query.OrderBy(q => q.QuestionId).ToListAsync();

            var selectedQuestions = await _context.QuestionInExams
                .Where(qe => qe.ExamId == examId)
                .Select(qe => qe.QuestionId)
                .ToListAsync();

            var model = new EditQuestionInExamViewModel
            {
                ExamId = exam.ExamId,
                ExamName = exam.ExamName,
                QuestionCount = exam.QuestionCount,
                SearchTerm = searchTerm,
                FilterLevel = filterLevel,
                Questions = questions.Select(q => new QuestionSelectModel
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Level = q.Level,
                    IsSelected = selectedQuestions.Contains(q.QuestionId)
                }).ToList()
            };

            ViewBag.LevelList = new SelectList(new[]
            {
                new { Value = "", Text = "All levels" },
                new { Value = "Easy", Text = "Easy" },
                new { Value = "Medium", Text = "Medium" },
                new { Value = "Hard", Text = "Hard" }
            }, "Value", "Text", filterLevel?.ToString());

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditQuestionInExam(EditQuestionInExamViewModel model)
        {
            var exam = await _context.Exams.FindAsync(model.ExamId);
            if (exam == null)
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });

            var selectedIdsFromForm = model.Questions
                .Where(q => q.IsSelected)
                .Select(q => q.QuestionId)
                .ToList();

            if (selectedIdsFromForm.Count != exam.QuestionCount)
            {
                ModelState.AddModelError("", $"Bạn phải chọn đúng {exam.QuestionCount} câu hỏi. Hiện tại bạn chọn {selectedIdsFromForm.Count} câu.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.LevelList = new SelectList(new[]
                {
                    new { Value = "", Text = "All levels" },
                    new { Value = "Easy", Text = "Easy" },
                    new { Value = "Medium", Text = "Medium" },
                    new { Value = "Hard", Text = "Hard" }
                }, "Value", "Text", model.FilterLevel?.ToString());

                var questionsQuery = _context.Questions
                    .Where(q => q.SubjectId == exam.SubjectId);

                if (!string.IsNullOrEmpty(model.SearchTerm))
                    questionsQuery = questionsQuery.Where(q => q.QuestionText.Contains(model.SearchTerm));

                if (model.FilterLevel.HasValue)
                    questionsQuery = questionsQuery.Where(q => q.Level == model.FilterLevel.Value);

                var questions = await questionsQuery.OrderBy(q => q.QuestionId).ToListAsync();

                model.ExamName = exam.ExamName;
                model.QuestionCount = exam.QuestionCount;

                model.Questions = questions.Select(q => new QuestionSelectModel
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Level = q.Level,
                    IsSelected = selectedIdsFromForm.Contains(q.QuestionId)
                }).ToList();

                return View(model);
            }

            var oldQuestions = _context.QuestionInExams.Where(qe => qe.ExamId == exam.ExamId);
            _context.QuestionInExams.RemoveRange(oldQuestions);

            foreach (var qId in selectedIdsFromForm)
            {
                _context.QuestionInExams.Add(new tblQuestionInExam
                {
                    ExamId = exam.ExamId,
                    QuestionId = qId,
                    CreatedAt = DateTime.Now,
                    CreatedBy = "Admin"
                });
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật câu hỏi thành công!";
            return RedirectToAction("EditQuestionInExam", new { examId = exam.ExamId });
        }

        public IActionResult Delete(int? examId)
        {
            if (examId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đề thi !";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            var exam = _context.Exams.Find(examId);

            if (exam == null)
            {
                TempData["ErrorMessage"] = "Đề thi không tồn tại !";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            return View(exam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromBody] DeleteExamRequest request)
        {
            if (request == null || request.Id <= 0)
                return BadRequest();

            var exam = _context.Exams.FirstOrDefault(u => u.ExamId == request.Id);
            if (exam == null)
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });

            _context.Exams.Remove(exam);
            _context.SaveChanges();

            return Ok();
        }
    }
    
    public class DeleteExamRequest
    {
        public int Id { get; set; }
    }
}