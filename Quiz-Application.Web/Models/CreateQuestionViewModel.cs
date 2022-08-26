using Quiz_Application.Services.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Quiz_Application.Web.Models
{
    public class CreateQuestionViewModel
    {
        [Required]
        public int ExamID { get; set; }
        [Required]
        public int QuestionType { get; set; }  //MCQ-1      
        [Required]
        public string QuestionText { get; set; }
        [Required]
        public string ChoiceOne { get; set; }
        [Required]
        public string ChoiceTwo { get; set; }
        [Required]
        public string ChoiceThree { get; set; }
        [Required]
        public string ChoiceFour { get; set; }
        [Required]
        [DisplayName("Answer No.")]
        [Range(1,4)]
        public int ChoiceNo { get; set; }
    }
}
