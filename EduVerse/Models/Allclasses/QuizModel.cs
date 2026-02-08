using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EduVerse.Models.Allclasses
{
    public class QuizModel
    {
        public class QuizMaterialModel
        {
            public int CourseId { get; set; }
            public HttpPostedFileBase UploadedFile { get; set; }
            public string FilePath { get; set; }
            public string FileType { get; set; }
            public string Title { get; set; }
        }

        public class QuizQuestionDto
        {
            public string Question { get; set; }
            public string OptionA { get; set; }
            public string OptionB { get; set; }
            public string OptionC { get; set; }
            public string OptionD { get; set; }
            public string Answer { get; set; } 
        }

        public class SaveQuizRequest
        {
            public int CourseId { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public int DurationInMinutes { get; set; }
            public List<QuizQuestionModel> Questions { get; set; }
        }

        public class QuizQuestionModel
        {
            public string QuestionText { get; set; }
            public string OptionA { get; set; }
            public string OptionB { get; set; }
            public string OptionC { get; set; }
            public string OptionD { get; set; }
            public string CorrectOption { get; set; }
        }
    }
}