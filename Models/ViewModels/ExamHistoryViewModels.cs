using System;
using System.Collections.Generic;

namespace LuyenThiTracNghiem.Models.ViewModels
{
    public class ExamHistoryViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public List<ExamHistoryItemViewModel> Attempts { get; set; } = new();
    }

    public class ExamHistoryItemViewModel
    {
        public int AttemptId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public DateTime AttemptTime { get; set; }
        public decimal? Score { get; set; }
        public decimal? PercentScore { get; set; }
        public TimeSpan? Duration { get; set; }
        public bool IsPassed { get; set; }
    }
}
