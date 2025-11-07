using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LuyenThiTracNghiem. Components
{
    [ViewComponent(Name = "TopStudents")]
    public class TopStudentsComponent : ViewComponent
    {
        private readonly DataContext _context;

        public TopStudentsComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listTopStudents = (from u in _context.Users
                                   where (u.Status == true)
                                   orderby u.UserId descending
                                   select u).Take(2).ToList();

            return await Task.FromResult((IViewComponentResult)View("Default", listTopStudents));
        }
    }
}