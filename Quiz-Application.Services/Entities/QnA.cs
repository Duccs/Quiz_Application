using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz_Application.Services.Entities
{
    public class QnA
    {
        public int ExamID { get; set; }
        public string Exam { get; set; }
        public List<QuestionDetails>  questions { get; set; }

        private Random rng = new Random();

        private List<T> Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public void RandomOrder(List<QuestionDetails> questions)
        {
            this.questions = Shuffle(questions);
        }
    }
    public class QuestionDetails
    {       
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public int QuestionType { get; set; }
        public List<OptionDetails> options { get; set; }
        public AnswerDetails answer { get; set; }
    }
    public class OptionDetails
    {
        public int OptionID { get; set; }
        public string Option { get; set; }    
    }
    public class AnswerDetails
    {
        public int AnswarID { get; set; }
        public int OptionID { get; set; }
        public string Answar { get; set; }
    }


}
