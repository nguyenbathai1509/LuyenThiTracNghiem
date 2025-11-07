using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LuyenThiTracNghiem.Areas.Admin.Models
{
    [Table("tblSubject")]
    public class tblSubject
    {
        [Key]
        [StringLength(10)]
        public string SubjectId { get; set; } = null!;

        [StringLength(255)]
        public string SubjectName { get; set; } = null!;

        public string? Description { get; set; }

        [StringLength(500)]
        public string? Image { get; set; }

        public bool Status { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string CreatedBy { get; set; } = "Admin";

        public DateTime? UpdatedAt { get; set; }

        [StringLength(255)]
        public string? UpdatedBy { get; set; }

        public virtual ICollection<tblQuestion> Questions { get; set; } = new List<tblQuestion>();
    }
}