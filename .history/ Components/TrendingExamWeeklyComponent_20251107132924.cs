using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LuyenThiTracNghiem. Components
{
    [ViewComponent(Name = "TrendingExamWeekly")]
    public class TrendingExamWeeklyComponent : ViewComponent
    {
        private readonly DataContext _context;

        public TrendingExamWeeklyComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listTrendingExam = (from exam in _context.Exams
                                        join attempt in _context.ExamAttempts on exam.ExamId equals attempt.ExamId
                                        where exam.Status == true && attempt.IsCompleted == true
                                        group attempt by new { exam.ExamId, exam.ExamName, exam.Image, exam.SubjectId } into g
                                        orderby g.Count() descending
                                        select new
                                        {
                                            g.Key.ExamId,
                                            g.Key.ExamName,
                                            g.Key.Image,
                                            g.Key.SubjectId,
                                            AttemptCount = g.Count()
                                        })
                                        .Take(7)
                                        .ToList();

            return await Task.FromResult((IViewComponentResult)View("Default", listTrendingExam));
        }
    }
}