using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LuyenThiTracNghiem.Models
{
    [Table("tblUser")]
    public class tblUser
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string UserCode { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(255)]
        public string FullName { get; set; } = null!;

        public DateTime? BirthDate { get; set; }

        [Required]
        [MaxLength(10)]
        public string Gender { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        [MaxLength(10)]
        public string? PhoneNumber { get; set; }

        [MaxLength(255)]
        public string? Email { get; set; }

        [MaxLength(255)]
        public string? Avatar { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Số dư không được âm.")]
        public decimal Balance { get; set; } = 0;

        public DateTime? LastLogin { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public bool IsPhoneVerified { get; set; } = false;

        [Required]
        public byte Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool Status { get; set; } = true;

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}