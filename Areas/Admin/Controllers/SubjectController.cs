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
    public class SubjectController : Controller
    {
        private readonly DataContext _context;
        public SubjectController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var query = _context.Subjects.AsQueryable();

            int totalItems = await query.CountAsync();

            var subjects = await query
                .OrderBy(s => s.SubjectId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new PagedResult<tblSubject>
            {
                Items = subjects,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        public IActionResult Search(string searchTerm, int page = 1)
        {
            int pageSize = 5;
            var query = _context.Subjects.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => s.SubjectName.Contains(searchTerm));
            }

            var totalItems = query.Count();
            var subjects = query
                .OrderBy(s => s.SubjectName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new PagedResult<tblSubject>
            {
                Items = subjects,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return PartialView("_SubjectListPartial", model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(tblSubject subject, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(subject);
            }

            bool sjIDExists = _context.Subjects.Any(s => s.SubjectId == subject.SubjectId);
            if (sjIDExists)
            {
                ModelState.AddModelError("ExamId", "Mã môn học đã tồn tại !");
                return View(subject);
            }

            bool sjNameExists = _context.Subjects.Any(s => s.SubjectName == subject.SubjectName);
            if (sjNameExists)
            {
                ModelState.AddModelError("SubjectName", "Tên môn học đã tồn tại !");
                return View(subject);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/images/img-subject", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }
                subject.Image = fileName;
            }

            subject.CreatedAt = DateTime.Now;
            subject.CreatedBy = "admin";
            _context.Subjects.Add(subject);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Thêm môn học thành công!";
            return RedirectToAction("Create");
        }

        public IActionResult Edit(string? SubjectId)
        {
            if (SubjectId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy môn học!";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            var sj = _context.Subjects.Find(SubjectId);

            if (sj == null)
            {
                TempData["ErrorMessage"] = "Môn học không tồn tại !";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            return View(sj);
        }

        [HttpPost]
        public IActionResult Edit(tblSubject subject, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(subject);
            }

            var oldSubject = _context.Subjects.FirstOrDefault(s => s.SubjectId == subject.SubjectId);
            if (oldSubject == null)
            {
                TempData["ErrorMessage"] = "Môn học không tồn tại !";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            bool sjNameExists = _context.Subjects
                .Any(s => s.SubjectName == subject.SubjectName && s.SubjectId != subject.SubjectId);
            if (sjNameExists)
            {
                ModelState.AddModelError("SubjectName", "Tên môn học đã tồn tại !");
                return View(subject);
            }

            oldSubject.SubjectName = subject.SubjectName;
            oldSubject.Description = subject.Description;
            oldSubject.Status = subject.Status;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(oldSubject.Image))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/images/img-subject", oldSubject.Image);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                var fileName = Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin/images/img-subject", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }
                oldSubject.Image = fileName;
            }

            oldSubject.UpdatedAt = DateTime.Now;
            oldSubject.UpdatedBy = "admin";

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Chỉnh sửa thông tin môn học thành công!";

            return RedirectToAction("Edit", new { SubjectId = subject.SubjectId });
        }

        public IActionResult Delete(string? subjectId)
        {
            if (subjectId == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy môn học !";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            var subject = _context.Subjects.Find(subjectId);

            if (subject == null)
            {
                TempData["ErrorMessage"] = "Môn học không tồn tại !";
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });
            }

            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromBody] DeleteSubjectRequest request)
        {
            if (request == null || request.Id == string.Empty)
                return BadRequest();

            var subject = _context.Subjects.FirstOrDefault(u => u.SubjectId == request.Id);
            if (subject == null)
                return RedirectToAction("NotFoundPage", "Error", new { area = "Admin" });

            _context.Subjects.Remove(subject);
            _context.SaveChanges();

            return Ok();
        }
    }
    public class DeleteSubjectRequest
    {
        public string? Id { get; set; }
    }
}