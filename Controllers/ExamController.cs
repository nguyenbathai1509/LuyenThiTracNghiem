using System;
using System.Linq;
using LuyenThiTracNghiem.Areas.Admin.Models;
using LuyenThiTracNghiem.Models;
using LuyenThiTracNghiem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuyenThiTracNghiem.Controllers
{
    public class ExamController : Controller
    {
        private readonly DataContext _context;

        public ExamController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1, int pageSize = 6)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, Math.Min(pageSize, 50));

            var examsQuery = _context.Exams
                .Where(e => e.Status)
                .Include(e => e.Subject);

            var totalItems = examsQuery.Count();

            var items = examsQuery
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.ExamId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new ExamListItemViewModel
                {
                    ExamId = e.ExamId,
                    ExamName = e.ExamName,
                    SubjectName = e.Subject != null ? e.Subject.SubjectName : string.Empty,
                    QuestionCount = e.QuestionCount,
                    DurationMinutes = e.DurationMinutes,
                    ExamFee = e.ExamFee,
                    Image = e.Image,
                    CreatedAt = e.CreatedAt
                })
                .ToList();

            var model = new PagedResult<ExamListItemViewModel>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };

            return View(model);
        }
    }
}
