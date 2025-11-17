using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LuyenThiTracNghiem.Models
{
    [Table("tblAttemptAnswer")]
    public class tblAttemptAnswer
    {
        []
        public int AttemptAnswerId { get; set; }
        public int AttemptId { get; set; }
        public int QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public bool IsCorrect { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Navigation
        public ExamAttempt Attempt { get; set; }
        public Question Question { get; set; }
        public Answer? Answer { get; set; }
    }
}