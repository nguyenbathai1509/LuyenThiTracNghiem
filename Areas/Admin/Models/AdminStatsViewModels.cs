using System.Collections.Generic;

namespace LuyenThiTracNghiem.Areas.Admin.Models
{
    public class AdminStatsViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalExams { get; set; }
        public int TotalCompletedAttempts { get; set; }
        public List<AdminTopExamStatsViewModel> TopExams { get; set; } = new();
        public List<AdminTopUserDepositViewModel> TopDepositUsers { get; set; } = new();
    }

    public class AdminTopExamStatsViewModel
    {
        public string ExamName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int AttemptCount { get; set; }
        public decimal AvgScore { get; set; }
    }

    public class AdminTopUserDepositViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int DepositCount { get; set; }
    }
}
