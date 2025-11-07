using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuyenThiTracNghiem.Areas.Admin.Models
{
    [Table("tblExamAttempt")]
    public class tblExamAttempt
    {
        [Key]
        public int AttemptId { get; set; }

        // Khóa ngoại
        [Required]
        public int ExamId { get; set; }           // FK -> tblExam

        public int? UserId { get; set; }          // FK -> tblUser (nếu có)

        // Kết quả làm bài
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Score { get; set; }       // Điểm

        public int? CorrectCount { get; set; }    // Số câu đúng
        public int? WrongCount { get; set; }      // Số câu sai
        public int? UnansweredCount { get; set; } // Số câu bỏ trống

        [Column(TypeName = "decimal(5,2)")]
        public decimal? PercentScore { get; set; } // % điểm đạt được

        // Thời gian & trạng thái
        [Required]
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;  // Bắt đầu (UTC)

        public DateTime? FinishedAt { get; set; }                   // Kết thúc (UTC)

        public int? DurationSeconds { get; set; }                   // Thời gian làm thực tế
        public int? TimeLimitSeconds { get; set; }                  // Giới hạn thời gian làm bài

        [Required]
        public bool IsCompleted { get; set; } = false;              // 1 = hoàn thành, 0 = bỏ dở

        // Audit
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Tạo lúc nào

        [StringLength(255)]
        public string? CreatedBy { get; set; }                      // Người tạo

        public DateTime? UpdatedAt { get; set; }                    // Cập nhật lúc nào

        [StringLength(255)]
        public string? UpdatedBy { get; set; }                      // Người cập nhật

        // Navigation Properties (tuỳ chọn, nếu có)
        [ForeignKey("ExamId")]
        public virtual tblExam? Exam { get; set; }

        [ForeignKey("UserId")]
        public virtual tblU? User { get; set; }
    }
}
