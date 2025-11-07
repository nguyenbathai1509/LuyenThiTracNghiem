using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LuyenThiTracNghiem.Areas.Admin.Models
{
    [Table("tblAnswer")]
    public class tblAnswer
    {
        [Key]
        public int AnswerId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        public string AnswerText { get; set; } = null!;

        [Required]
        public bool IsCorrect { get; set; } = true;

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

        [ForeignKey("QuestionId")]
        public virtual tblQuestion Question { get; set; } = null!;
    }
}