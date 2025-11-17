using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuyenThiTracNghiem.Models.ViewModels
{
    public class SubmitExamModel
    {
        public int AttemptId { get; set; }
        public List<SubmitAnswerItem> Answers { get; set; } = new List<SubmitAnswerItem>();
    }

    public class SubmitAnswerItem
    {
        public int QuestionId { get; set; }
        public int? SelectedAnswerId { get; set; } // null nếu không chọn
    }
}