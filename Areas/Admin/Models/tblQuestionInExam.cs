using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LuyenThiTracNghiem.Areas.Admin.Models
{
    [Table("tblQuestionInExam")]
    public class tblQuestionInExam
    {
        [Key]
        public int QuestionInExamId { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [StringLength(255)]
        public string CreatedBy { get; set; } = "Admin";

        public DateTime? UpdatedAt { get; set; }

        [StringLength(255)]
        public string? UpdatedBy { get; set; }

        [ForeignKey("ExamId")]
        public virtual tblExam Exam { get; set; } = null!;

        [ForeignKey("QuestionId")]
        public virtual tblQuestion Question { get; set; } = null!;
    }
}