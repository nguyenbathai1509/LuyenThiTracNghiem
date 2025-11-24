using System;

namespace LuyenThiTracNghiem.Models.ViewModels
{
    public class ExamListItemViewModel
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string? SubjectName { get; set; }
        public int QuestionCount { get; set; }
        public int DurationMinutes { get; set; }
        public decimal ExamFee { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
