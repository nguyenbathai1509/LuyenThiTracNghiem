using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuyenThiTracNghiem. Components
{
    [ViewComponent(Name = "Balance")]
    public class BalanceComponent : ViewComponent
    {
        private readonly DataContext _context;

        public BalanceComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            decimal balance = 0m;

            if (userId.HasValue)
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == userId.Value);

                if (user != null)
                {
                    balance = user.Balance;
                }
            }

            return View("Default", balance);
        }
    }
}