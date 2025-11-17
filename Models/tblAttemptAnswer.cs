using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LuyenThiTracNghiem.Areas.Admin.Models;

namespace LuyenThiTracNghiem.Models
{
    [Table("tblAttemptAnswer")]
    public class tblAttemptAnswer
    {
        [Key]
        public int AttemptAnswerId { get; set; }

        public int AttemptId { get; set; }
        public int QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public bool IsCorrect { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public tblAttemptAnswer()
        {
            CreatedAt = DateTime.Now;
        }

        [ForeignKey("AttemptId")]
        public virtual tblExamAttempt Attempt { get; set; } = null!;

        [ForeignKey("QuestionId")]
        public virtual tblQuestion Question { get; set; } = null!;

        [ForeignKey("AnswerId")]
        public virtual tblAnswer? Answer { get; set; }
    }
}
