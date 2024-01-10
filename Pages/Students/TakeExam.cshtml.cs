// TakeExamModel.cshtml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Test.Me.models;

namespace Test.Me.Pages.Students
{
    public class TakeExamModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TakeExamModel> _logger;
        private readonly ApplicationDbContext _context;

        [BindProperty(SupportsGet = true)]
        public int? Qid { get; set; }

        public Quiz Exam { get; set; }

        // Add a list to store the questions and student answers
        public List<QuestionViewModel> Questions { get; set; }

        public int RemainingTime { get; set; }
        public StringValues QuestionsStudentAnswer { get; private set; }

        List<string> questionsStudentAnswers = new List<string>();


        public TakeExamModel(IConfiguration configuration, ILogger<TakeExamModel> logger, ApplicationDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
            Questions = new List<QuestionViewModel>();
        }

        public IActionResult OnGet(int? qid)
        {
            // Check if Qid is not provided in the query parameters
            if (qid == null)
            {
                // Redirect to the Index1 page if Qid is not provided
                return RedirectToPage("/Index1");
            }
            // Retrieve the quiz from the database based on Qid
            var quiz = _context.Quiz.FirstOrDefault(q => q.Qid == qid);
            // Set the Qid property to the provided value
            Qid = qid;

            // Retrieve the exam details based on Qid
            Exam = _context.Quiz.Find(Qid);

            // Check if the exam is not found
            if (Exam == null)
            {
                // Redirect to the Index2 page if the exam is not found
                return RedirectToPage("/Index2");
            }

            RemainingTime = quiz.Qtime;


            // Retrieve questions for the exam based on Qid
            Questions = _context.Question
                .Where(q => q.Qid == Qid)
                .Select(q => new QuestionViewModel
                {
                    Quid = q.Quid,
                    Qutext = q.Qutext,
                    IsMultipleChoice = q.Qutype,
                    Firstop = q.Firstop ?? "N/A", // Use "N/A" if null
                    Secondop = q.Secondop ?? "N/A",
                    Thirdop = q.Thirdop ?? "N/A",
                    Fourthop = q.Fourthop ?? "N/A",
                    Mark = q.Qumark,
                    // Include other properties as needed
                })
                .ToList();

            // Serialize Questions to JSON 
            HttpContext.Session.SetString("Questions", JsonConvert.SerializeObject(Questions));

            // Return the page
            return Page();
        }

        public async Task<IActionResult> OnPostSubmitExam()
        {
            try
            {
                // Log statement for debugging
                _logger.LogInformation("OnPostSubmitExam method called.");

                // Get the current student id from the session
                var studentId = HttpContext.Session.GetInt32("StudentId");


                // Check if the student is not logged in
                if (studentId == null)
                {
                    // Redirect to the Login page if the student is not logged in
                    return RedirectToPage("/Login");
                }

                // Retrieve Questions from TempData
                var questionsJson = HttpContext.Session.GetString("Questions");
                _logger.LogError($"questionJson is {questionsJson}");


                // Check if Questions data is not found in TempData
                if (string.IsNullOrEmpty(questionsJson))
                {
                    // Log an error and return the page
                    _logger.LogError("Questions data not found in TempData.");
                    return RedirectToPage("/fu");
                }

                // Deserialize Questions from JSON
                Questions = JsonConvert.DeserializeObject<List<QuestionViewModel>>(questionsJson);

                // Initialize the total mark
                int totalMark = 0;
                int counter = 0;
                // Iterate through each question and process the answers
                foreach (var question in Questions)
                {
                    // Log the processing of the current question
                    _logger.LogInformation($"Processing question {question.Quid}");

                    _logger.LogInformation($"Form values: {string.Join(", ", Request.Form.Select(kv => $"{kv.Key}={kv.Value}"))}");

                    string questionAnswerKey = $"ans {counter}";

                        _logger.LogInformation("is multiiiiiiiii");
                        _logger.LogInformation($"ans {counter}");

                        // Retrieve the answer(s) from the form
                        string[] currentQuestionAnswers = Request.Form[questionAnswerKey];
                    if (currentQuestionAnswers == null || !currentQuestionAnswers.Any())
                    {
                        currentQuestionAnswers = new[] { " " }; // Assign a default value
                    }

                    // Handle the case where the answers are null
                    currentQuestionAnswers ??= Array.Empty<string>();

                        // Log the student's answer(s) for the current question
                        _logger.LogInformation($"Student's answer(s) for question {question.Quid}: {string.Join(", ", currentQuestionAnswers)}");

                        // Store the answers in the list
                        questionsStudentAnswers.AddRange(currentQuestionAnswers);


                    // Retrieve the correct answer from the database based on Quid
                    var correctAnswer = _context.Question
                        .Where(q => q.Quid == question.Quid)
                        .Select(q => q.Qurightanswer)
                        .FirstOrDefault();

                    // Log the correct answer for the current question
                    _logger.LogInformation($"Correct answer for question {question.Quid}: {correctAnswer}");

                    // Check if the correct answer is null, and skip processing if so
                    if (correctAnswer == null)
                    {
                        _logger.LogError($"Correct answer for question {question.Quid} is null. Skipping.");
                        continue; // Skip processing this question
                    }

                    // Log the student's answer for the current question
                    _logger.LogInformation($"Student's answer for question {question.Quid}: {questionsStudentAnswers[counter]}");

                    // Check if the student's answer is correct
                    bool isCorrect = questionsStudentAnswers[counter] == correctAnswer;
                    _logger.LogInformation($"Is the answer correct? {isCorrect}");
                    _logger.LogInformation($"counter is  {counter}");
                    _logger.LogInformation($"student answer is  {questionsStudentAnswers[counter]}");

                    string studentAnswer = questionsStudentAnswers[counter] ?? " ";
                    _logger.LogInformation($"counter is  {counter}");

                    // Update the Student_Question table with the student's answer and state
                    _context.Student_Question.Add(new Student_Question
                    {
                        Sid = studentId.Value,
                        Quid = question.Quid,
                        Sanswer = studentAnswer,
                        Answrestate = isCorrect ? "right" : "wrong"
                    });

                    // Log the addition of the entry to Student_Question for the current question
                    _logger.LogInformation($"Added entry to Student_Question for question {question.Quid}");

                    // Update the total mark if the answer is correct
                    if (isCorrect)
                    {
                        totalMark += question.Mark;
                    }
                    counter++;
                }

                // Log the total mark for the exam
                _logger.LogInformation($"Total mark for the exam: {totalMark}");

                // Update the Student_Quiz table with the student's mark
                _context.Student_Quiz.Add(new Student_Quiz
                {
                    Sid = studentId.Value,
                    Qid = Qid.Value,
                    Smark = totalMark
                });

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Redirect to a page indicating the exam submission
                return RedirectToPage("/Students/ExamSubmitted");
            }
            catch (Exception ex)
            {
                // Log an error if an exception occurs during exam submission
                _logger.LogError(ex, "Error submitting the exammmm.");
                // Return the page
                return Page();
            }
        }

    }

    // Create a ViewModel for question data
    public class QuestionViewModel
    {
        public int Quid { get; set; }
        public string Qutext { get; set; }
        public bool IsMultipleChoice { get; set; }
        public string QuestionType => IsMultipleChoice ? "MultipleChoice" : "SimpleAnswer";
        public string Firstop { get; set; }
        public string Secondop { get; set; }
        public string Thirdop { get; set; }
        public string Fourthop { get; set; }
        public int Mark { get; set; }
        public int Qumark { get; set; }

        [BindProperty]
        public bool Qutype { get; set; } // "MultipleChoice" or "SimpleAnswer"
    }
}
