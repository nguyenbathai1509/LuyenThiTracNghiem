using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuyenThiTracNghiem. Components
{
    [ViewComponent(Name = "NewExam")]
    public class NewExamComponent : ViewComponent
    {
        private readonly DataContext _context;

        public NewExamComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listNewExam = await (
            from exam in _context.Exams.AsNoTracking()
            join subject in _context.Subjects.AsNoTracking()
                on exam.SubjectId equals subject.SubjectId
            where exam.Status == true
            orderby exam.CreatedAt descending, exam.ExamId descending
            select new
            {
                exam.ExamId,
                exam.ExamName,
                exam.Image,
                exam.SubjectId,
                SubjectName = subject.SubjectName,
                exam.CreatedAt
            })
            .Take(6)
            .ToListAsync();

            return await Task.FromResult((IViewComponentResult)View("Default", listNewExam));
        }
    }
}