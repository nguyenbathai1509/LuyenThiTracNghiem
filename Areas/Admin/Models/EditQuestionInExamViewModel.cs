using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LuyenThiTracNghiem.Areas.Admin.Models
{
    public class EditQuestionInExamViewModel
    {
        public int ExamId { get; set; }

        public string ExamName { get; set; } = string.Empty;

        public int QuestionCount { get; set; }

        public string? SearchTerm { get; set; }

        public QuestionLevel? FilterLevel { get; set; }

        [Required(ErrorMessage = "Danh sách câu hỏi không được để trống")]
        public List<QuestionSelectModel> Questions { get; set; } = new List<QuestionSelectModel>();
    }

    public class QuestionSelectModel
    {
        public int QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public QuestionLevel Level { get; set; }

        public bool IsSelected { get; set; } = false;
    }
}