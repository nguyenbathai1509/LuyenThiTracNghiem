using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenThiTracNghiem.Areas.Admin.Models;
using LuyenThiTracNghiem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var attemptStats = await _context.ExamAttempts
                .Where(a => a.IsCompleted && a.UserId != null && a.PercentScore != null)
                .GroupBy(a => a.UserId!.Value)
                .Select(g => new
                {
                    UserId = g.Key,
                    AvgPercent = g.Average(x => x.PercentScore ?? 0),
                    Attempts = g.Count(),
                    LastAttempt = g.Max(x => x.FinishedAt ?? x.CreatedAt)
                })
                .OrderByDescending(x => x.AvgPercent)
                .ThenByDescending(x => x.Attempts)
                .ThenByDescending(x => x.LastAttempt)
                .Take(5)
                .ToListAsync();

            var userIds = attemptStats.Select(s => s.UserId).ToList();

            var users = await _context.Users
                .Where(u => u.Status && userIds.Contains(u.UserId))
                .ToListAsync();

            var topUsers = attemptStats
                .Join(users, s => s.UserId, u => u.UserId, (s, u) => new { Stats = s, User = u })
                .OrderByDescending(x => x.Stats.AvgPercent)
                .ThenByDescending(x => x.Stats.Attempts)
                .ThenByDescending(x => x.Stats.LastAttempt)
                .Select(x => x.User)
                .ToList();

            return View("Default", topUsers);
        }
    }
}
