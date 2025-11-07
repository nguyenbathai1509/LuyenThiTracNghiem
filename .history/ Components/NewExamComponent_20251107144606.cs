using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;

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
            var listNewExam = (from exam in _context.Exams
                       join subject in _context.Subjects on exam.SubjectId equals subject.SubjectId
                       join attempt in _context.ExamAttempts on exam.ExamId equals attempt.ExamId
                       where exam.Status == true && attempt.IsCompleted == true
                       group new { exam, subject } by new
                       {
                           exam.ExamId,
                           exam.ExamName,
                           exam.Image,
                           exam.SubjectId,
                           subject.SubjectName
                       } into g
                       orderby g.Count() descending
                       select new
                       {
                           g.Key.ExamId,
                           g.Key.ExamName,
                           g.Key.Image,
                           g.Key.SubjectId,
                           g.Key.SubjectName,
                           AttemptCount = g.Count()
                       })
                       .Take(7)
                       .ToList();

    return await Task.FromResult((IViewComponentResult)View("Default", listNewExam));
        }
    }
}