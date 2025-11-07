using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LuyenThiTracNghiem.Areas.Admin.Models
{
    [Table("tblQuestion")]
    public class tblQuestion
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        [StringLength(10)]
        public string SubjectId { get; set; } = null!;

        [Required]
        public string QuestionText { get; set; } = null!;

        [Required]
        public QuestionLevel Level { get; set; } = QuestionLevel.Easy;

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
        public virtual tblSubject Subject { get; set; } = null!;
        
        public virtual ICollection<tblAnswer> Answers { get; set; } = new List<tblAnswer>();
    }

    
}