using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LuyenThiTracNghiem.Areas.Admin.Models
{
    [Table("tblAdminMenu")]
    public class tblAdminMenu
    {
        [Key]
        public long AdminMenuID { get; set; }

        [Required]
        [StringLength(50)]
        public string ItemName { get; set; } = null!;

        public int? ItemLevel { get; set; }

        public int? ParentLevel { get; set; }

        public int? ItemOrder { get; set; }

        [StringLength(50)]
        public string? ItemTarget { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(20)]
        public string? AreaName { get; set; }

        [StringLength(20)]
        public string? ControllerName { get; set; }

        [StringLength(20)]
        public string? ActionName { get; set; }

        [StringLength(50)]
        public string? Icon { get; set; }

        [StringLength(50)]
        public string? IdName { get; set; }
    }
}