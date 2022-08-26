using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quiz_Application.Web.Authentication;
using Quiz_Application.Services.Entities;
using Quiz_Application.Web.Models;
using Quiz_Application.Services.Repository.Interfaces;
using Quiz_Application.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quiz_Application.Web.Extensions;

namespace Quiz_Application.Web.Controllers
{    
    [BasicAuthentication]   
    public class ExamController : Controller
    {
        private readonly QuizDBContext _context;
        private readonly ILogger<ExamController> _logger;
        private readonly IExam<Services.Entities.Exam> _exam;
        private readonly IQuestion<Services.Entities.Question> _question;
        private readonly IResult<Services.Entities.Result> _result;
        public ExamController(ILogger<ExamController> logger, IExam<Services.Entities.Exam> exam, IQuestion<Services.Entities.Question> question, IResult<Services.Entities.Result> result, QuizDBContext context)
        {
            _context = context;
            _logger = logger;
            _exam = exam;
            _question = question;
            _result = result;
        }
             
        [HttpGet]
        [Route("~/api/Exams")]
        public async Task<IActionResult> Exams()
        {           
            try
            {
                IEnumerable<Exam> lst = await _exam.GetExamList();               
                return Ok(lst.ToList());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally 
            {
            }
        }

        [HttpGet]
        [Route("~/api/Exam/{ExamID?}")]
        public async Task<IActionResult> Exam(int ExamID)
        {
            try
            {
                Exam exm = await _exam.GetExam(ExamID);
                return Ok(exm);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally 
            {
            }
        }

        // GET: Exam/CreateExam
        public IActionResult CreateExam()
        {
            ViewBag.Candidate = HttpContext.Session.GetObjectFromJson<Candidate>("AuthenticatedUser");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExam([Bind("ExamID,Name,FullMarks,Duration")] Exam exam)
        {
            ViewBag.Candidate = HttpContext.Session.GetObjectFromJson<Candidate>("AuthenticatedUser");
            if (ModelState.IsValid)
            {
                _context.Add(exam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CreateExam));
            }
            return View(exam);
        }

        public IActionResult CreateQuestion()
        {
            ViewBag.Candidate = HttpContext.Session.GetObjectFromJson<Candidate>("AuthenticatedUser");
            ViewBag.ExamID = new SelectList(_context.Exam, "ExamID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestion([Bind("ExamID,QuestionType,QuestionText,ChoiceOne,ChoiceTwo,ChoiceThree,ChoiceFour,ChoiceNo")] CreateQuestionViewModel model)
        {
            ViewBag.Candidate = HttpContext.Session.GetObjectFromJson<Candidate>("AuthenticatedUser");
            if (ModelState.IsValid)
            {
                Question question = new Question()
                {
                    ExamID = model.ExamID,
                    QuestionType = model.QuestionType,
                    DisplayText = model.QuestionText,
                };
                _context.Add(question);
                await _context.SaveChangesAsync();
                Choice choiceOne = new Choice()
                {
                    QuestionID = question.QuestionID,
                    DisplayText = model.ChoiceOne
                };
                Choice choiceTwo = new Choice()
                {
                    QuestionID = question.QuestionID,
                    DisplayText = model.ChoiceTwo
                };
                Choice choiceThree = new Choice()
                {
                    QuestionID = question.QuestionID,
                    DisplayText = model.ChoiceThree
                };
                Choice choiceFour = new Choice()
                {
                    QuestionID = question.QuestionID,
                    DisplayText = model.ChoiceFour
                };
                _context.Add(choiceOne);
                _context.Add(choiceTwo);
                _context.Add(choiceThree);
                _context.Add(choiceFour);
                await _context.SaveChangesAsync();
                Choice answerChoice;
                switch (model.ChoiceNo)
                {
                    case 1:
                        answerChoice = choiceOne;
                        break;
                    case 2:
                        answerChoice = choiceTwo;
                        break;
                    case 3:
                        answerChoice = choiceThree;
                        break;
                    case 4:
                        answerChoice = choiceFour;
                        break;
                    default:
                        answerChoice = choiceOne;
                        break;
                }
                Answer answer = new Answer()
                {
                    QuestionID = question.QuestionID,
                    ChoiceID = answerChoice.ChoiceID,
                    DisplayText = answerChoice.DisplayText
                };
                _context.Add(answer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CreateQuestion));
            }
            return View(model);
        }

        public IActionResult CreateChoice()
        {
            ViewBag.Candidate = HttpContext.Session.GetObjectFromJson<Candidate>("AuthenticatedUser");
            ViewBag.QuestionID = new SelectList(_context.Question, "QuestionID", "DisplayText");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChoice([Bind("ChoiceID,QuestionID,DisplayText")] Choice choice)
        {
            ViewBag.Candidate = HttpContext.Session.GetObjectFromJson<Candidate>("AuthenticatedUser");
            if (ModelState.IsValid)
            {
                _context.Add(choice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CreateChoice));
            }
            return View(choice);
        }

        public IActionResult CreateAnswer()
        {
            ViewBag.Candidate = HttpContext.Session.GetObjectFromJson<Candidate>("AuthenticatedUser");
            ViewBag.QuestionID = new SelectList(_context.Question, "QuestionID", "DisplayText");
            ViewBag.ChoiceID = new SelectList(_context.Choice, "ChoiceID", "DisplayText");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAnswer([Bind("Sl_No,QuestionID,ChoiceID,DisplayText")] Answer answer)
        {
            ViewBag.Candidate = HttpContext.Session.GetObjectFromJson<Candidate>("AuthenticatedUser");
            if (ModelState.IsValid)
            {
                _context.Add(answer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CreateAnswer));
            }
            return View(answer);
        }

       

        [HttpGet]
        [Route("~/api/Questions/{ExamID?}")]
        public async Task<IActionResult> Questions(int ExamID)
        {
            try
            {
                QnA _obj = await _question.GetQuestionList(ExamID);
                _obj.RandomOrder(_obj.questions);
                return Ok(_obj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
            finally 
            {
            }
        }

        [HttpPost]
        [Route("~/api/Score")]       
        public async Task<IActionResult> Score(List<Option> objRequest)
        {
            int i = 0;
            bool IsCorrect = false;
            List<Result> objList = null;
            string _SessionID = null;
            try
            {               
                if (objRequest.Count > 0)
                {
                    _SessionID = Guid.NewGuid().ToString() + "-" + DateTime.Now;
                    objList = new List<Result>();
                    foreach (var item in objRequest)
                    {
                        if (item.AnswerID == item.SelectedOption)
                            IsCorrect = true;
                        else
                            IsCorrect = false;

                        Result obj = new Result()
                        {
                            CandidateID = item.CandidateID,
                            ExamID = item.ExamID,
                            QuestionID = item.QuestionID,
                            AnswerID = item.AnswerID,
                            SelectedOptionID = item.SelectedOption,
                            IsCorrent = IsCorrect,
                            SessionID= _SessionID,
                            CreatedBy = "SYSTEM",
                            CreatedOn = DateTime.Now
                        };
                        objList.Add(obj);
                    }
                    i = await _result.AddResult(objList);
                }
               
            }
            catch (Exception ex)
            {
                i = 0;
                throw new Exception(ex.Message, ex.InnerException);           
            }
            finally
            {                
            }
            return Ok(i);
        }
        
    }
}
