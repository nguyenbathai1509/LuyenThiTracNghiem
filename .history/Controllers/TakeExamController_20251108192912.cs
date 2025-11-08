using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            if (id == null)
                return NotFound();

            var questionInExam = _context.QuestionInExams.Where(q => q.ExamId == id).ToList();

            for
        }
    }
}