using System;
using System.Collections.Generic;
using LuyenThiTracNghiem.Areas.Admin.Models;

namespace LuyenThiTracNghiem.Models.ViewModels
{
    public class UserStatsViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int TotalAttempts { get; set; }
        public int TotalExamsTaken { get; set; }
        public decimal AvgScore { get; set; }
        public decimal AvgPercentScore { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalWrong { get; set; }
        public int TotalUnanswered { get; set; }
        public int TotalDeposits { get; set; }
        public int SuccessfulDeposits { get; set; }
        public int PendingDeposits { get; set; }
        public int FailedDeposits { get; set; }
        public decimal TotalDepositAmount { get; set; }
        public DateTime? LastDepositDate { get; set; }
        public decimal? LastDepositAmount { get; set; }
        public List<UserSubjectStatsViewModel> SubjectStats { get; set; } = new();
        public List<UserDifficultyStatsViewModel> DifficultyStats { get; set; } = new();
    }

    public class UserSubjectStatsViewModel
    {
        public string SubjectName { get; set; } = string.Empty;
        public int ExamsTaken { get; set; }
        public decimal AvgPercentScore { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalWrong { get; set; }
    }

    public class UserDifficultyStatsViewModel
    {
        public QuestionLevel Level { get; set; }
        public int TotalAnswered { get; set; }
        public int CorrectCount { get; set; }
        public decimal AccuracyPercent { get; set; }
    }
}
