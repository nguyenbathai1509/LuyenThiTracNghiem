using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuyenThiTracNghiem.Models.ViewModels
{
    public class ExamResultViewModel
{
    public string ExamName { get; set; }
    public double Score { get; set; }
    public int CorrectCount { get; set; }
    public int IncorrectCount { get; set; }
    public List<ResultQuestionDto> Questions { get; set; }
}

public class ResultQuestionDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public int? UserAnswerId { get; set; }
    public int CorrectAnswerId { get; set; }
    public List<AnswerDto> Answers { get; set; }
}

}