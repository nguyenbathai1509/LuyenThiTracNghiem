using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuyenThiTracNghiem.Areas.Admin.Models
{
    public class QuestionCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn môn học")]
        public string SubjectId { get; set; } = string.Empty;

        public int? QuestionId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung câu hỏi")]
        [StringLength(1000, ErrorMessage = "Nội dung câu hỏi không được vượt quá 1000 ký tự")]
        public string QuestionText { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn độ khó")]
        public QuestionLevel? Level { get; set; }

        public bool Status { get; set; } = false;

        [MinLength(4, ErrorMessage = "Vui lòng nhập đủ 4 đáp án")]
        public List<AnswerCreateModel> Answers { get; set; } = new List<AnswerCreateModel>
        {
            new AnswerCreateModel(),
            new AnswerCreateModel(),
            new AnswerCreateModel(),
            new AnswerCreateModel()
        };

        [Range(0, 3, ErrorMessage = "Vui lòng chọn đáp án đúng")]
        public int CorrectAnswer { get; set; }
    }

    public class AnswerCreateModel
    {
        [Required(ErrorMessage = "Vui lòng nhập nội dung đáp án")]
        [StringLength(500, ErrorMessage = "Đáp án không được vượt quá 500 ký tự")]
        public string AnswerText { get; set; } = null!;

        public bool IsCorrect { get; set; }
    }
}
