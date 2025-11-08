using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuyenThiTracNghiem.Models.ViewModels
{
    public class TakeExamViewModel
    {
        public class TakeExamViewModel
        {
            public int AttemptId { get; set; }
            public int ExamId { get; set; }
            public string ExamName { get; set; } = string.Empty;
            public int TimeRemainingSeconds { get; set; }
            public List<QuestionDto> Questions { get; set; } = new();
        }

        public class QuestionDto
        {
            public int QuestionId { get; set; }
            public string QuestionText { get; set; } = string.Empty;
            public List<AnswerDto> Answers { get; set; } = new();
        }

        public class AnswerDto
        {
            public int AnswerId { get; set; }
            public string AnswerText { get; set; } = string.Empty;
        }
    }
}