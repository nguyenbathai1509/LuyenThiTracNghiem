using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LuyenThiTracNghiem.Models
{
    [Table("tblPayment")]
    public class tblPayment
    {
        [Key]
        public long PaymentId { get; set; }

        [Required]
        [Display(Name = "Người nạp")]
        public int UserId { get; set; }

        [Required]
        [Display(Name = "Số tiền nạp (VNĐ)")]
        [Range(1000, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 1,000 VNĐ")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Success, Failed

        [Display(Name = "Mã giao dịch")]
        public string TransactionCode { get; set; } = string.Empty;

        [Display(Name = "Ngày thanh toán")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        public bool IsProcessed { get; set; } = false;

        [Display(Name = "Người tạo")]
        public string CreatedBy { get; set; } = string.Empty;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Người cập nhật")]
        public string? UpdatedBy { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedDate { get; set; }

        [ForeignKey("UserId")]
        public tblUser? User { get; set; }
    }
}