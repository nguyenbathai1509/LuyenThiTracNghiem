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
            var userId = HttpContext.Session.GetInt32("UserId");

            var user = _context.Users.Where(u => u.UserId == userId).FirstOrDefault();

            if (user == null)
                return NotFound();

            if (id == null)
                return NotFound();

            var exam = _context.Exams.Where(e => e.ExamId == id && e.Status == true).FirstOrDefault();

            if (exam == null)
                return NotFound();

            if()

            var questionInExam = _context.QuestionInExams.Where(q => q.ExamId == id).ToList();


        }
    }
}