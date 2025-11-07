using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuyenThiTracNghiem.Areas.Admin.Models
{
    [Table("tblExam")]
    public class tblExam
    {
        [Key]
        public int ExamId { get; set; }

        [Required]
        [StringLength(255)]
        public string ExamName { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public int QuestionCount { get; set; }

        [Required]
        public int DurationMinutes { get; set; }

        [Required]
        [StringLength(100)]
        public string ExamType { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ExamFee { get; set; } = 0;

        [StringLength(500)]
        public string? Image { get; set; }

        [Required]
        [StringLength(10)]
        public string SubjectId { get; set; } = null!;

        [Required]
        public bool Status { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [StringLength(255)]
        public string CreatedBy { get; set; } = "Admin";

        public DateTime? UpdatedAt { get; set; }

        [StringLength(255)]
        public string? UpdatedBy { get; set; }

        [ForeignKey("SubjectId")]
        public virtual tblSubject? Subject { get; set; } = null!;
    }
}
