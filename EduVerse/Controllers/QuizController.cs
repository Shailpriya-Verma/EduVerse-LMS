using EduVerse.Models.Allclasses;
using EduVerse.Models.BusinessLayer;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static EduVerse.Models.Allclasses.QuizModel;
using static System.Net.Mime.MediaTypeNames;


namespace EduVerse.Controllers
{
    public class QuizController : Controller
    {
        QuizBAL Qcls = new QuizBAL();


        #region UploadFile
        public ActionResult UploadFile()
        {
            if (Session["UserId"] == null || Session["RoleId"].Equals("3"))
            {
                return RedirectToAction("Login", "Account");
            }
            int teacherId = Convert.ToInt32(Session["UserId"]);
            DataTable dt = Qcls.cls_GetCoursesByInstructor(teacherId);
            ViewBag.CourseList = dt;
            return View();
        }
        #endregion


        #region Uploadmaterial

        [HttpPost]
        public ActionResult UploadMaterial(QuizMaterialModel model)
        {
                if (model.UploadedFile != null && model.UploadedFile.ContentLength > 0)
                {
                    string folderPath = Server.MapPath("~/Content/Uploads/QuizMaterials/");

                    Random rnd = new Random();
                    string fileName = rnd.Next(1000,9999)+System.IO.Path.GetFileName(model.UploadedFile.FileName);
                    string filePath = System.IO.Path.Combine(folderPath, fileName);
                    // To save pdf in folder .. currently no need    
                    model.UploadedFile.SaveAs(filePath);

                    model.FilePath = "/Content/Uploads/QuizMaterials/" + fileName;
                    model.FileType = System.IO.Path.GetExtension(fileName).ToLower();

                    TempData["Success"] = "File uploaded successfully!";
                    TempData["FilePath"] = model.FilePath;
                    TempData["FileType"] = model.FileType;

                    // Redirect to quiz generation step (next)
                    return RedirectToAction("CreateQuiz", new { courseId = model.CourseId });
                }

                return View();
        }
        #endregion Uploadmaterial


        #region CreateQuiz
        public ActionResult CreateQuiz(int courseId)
        {
            // Retrieve uploaded file info from TempData
            if (TempData["FilePath"] == null || TempData["FileType"] == null)
            {
                ViewBag.Error = "No uploaded file found. Please upload material first.";
                return View();
            }

            ViewBag.CourseId = courseId;
            ViewBag.FilePath = TempData["FilePath"].ToString();
            ViewBag.FileType = TempData["FileType"].ToString();

            return View();
        }

        #endregion CreateQuiz


        #region ExtractContent
        public string ExtractContent(string absolutePath,string FileType)
        {
            string extractedText="";

            if (FileType == ".pdf")
            {
                extractedText = ExtractTextFromPdf(absolutePath);
            }
            //else if (FileType == ".jpg" || FileType == ".jpeg" || FileType == ".png")
            //{
            //    // 🔹 For image-based text, use OCR (optional: Tesseract)
            //    extractedText = "This is a placeholder text extracted from image.";
            //}
            //else if (FileType == ".mp4")
            //{
            //    // 🔹 For videos, you’d need transcript extraction — optional for now
            //    extractedText = "This is a placeholder transcript from video.";
            //}
            return extractedText;
        }
        #endregion

        #region ExtractTextFromPdf
        private string ExtractTextFromPdf(string pdfPath)
        {
            StringBuilder text = new StringBuilder();

            using (PdfReader reader = new PdfReader(pdfPath))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
            }
            return text.ToString();
        }
        #endregion ExtractTextFromPdf


        #region GenerateQuiz
        [HttpPost]
        public async Task<JsonResult> GenerateQuiz(int CourseId, string FilePath, string FileType)
        {
            try
            {
                if (string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(FileType))
                    return Json(new { success = false, message = "Invalid file data." });

                // Step 1: Extract text
                string extractedText = "";
                string absolutePath = Server.MapPath(FilePath);

                extractedText = ExtractContent(absolutePath, FileType);

                if (string.IsNullOrWhiteSpace(extractedText))
                    return Json(new { success = false, message = "No readable text found in the file." });


                // Step 2: Ask AI to generate quiz
                var quizQuestions = await GenerateQuizFromGeminiAI(extractedText);

                // Step 3: Return result
                return Json(new { success = true, message = "Quiz generated successfully!", quiz = quizQuestions });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
        #endregion

        //#region GenerateQuizFromOpenAI

        //private async Task<List<QuizQuestionDto>> GenerateQuizFromGroqAI(string textContent)
        //{
        //    try
        //    {
        //        string apiKey = System.Configuration.ConfigurationManager.AppSettings["Groq_ApiKey"];

        //        using (var client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        //            var body = new
        //            {
        //                model = "llama-3.1-8b-instant",
        //                messages = new[]
        //                {
        //            new { role = "system", content = "Return ONLY VALID JSON. No explanation. No markdown. No extra text." },
        //            new { role = "user", content =
        //                "Generate EXACTLY 5 MCQs ONLY in this JSON format:\n" +
        //                "[{\"Question\":\"Q1?\",\"OptionA\":\"A\",\"OptionB\":\"B\",\"OptionC\":\"C\",\"OptionD\":\"D\",\"Answer\":\"A\"}]\n\n" +
        //                "Content:\n" + textContent
        //            }
        //        }
        //            };

        //            var json = JsonConvert.SerializeObject(body);
        //            var content = new StringContent(json, Encoding.UTF8, "application/json");

        //            var response = await client.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
        //            var respStr = await response.Content.ReadAsStringAsync();

        //            if (!response.IsSuccessStatusCode)
        //                return new List<QuizQuestionDto>
        //        {
        //            new QuizQuestionDto { Question = "Groq API error: " + respStr }
        //        };

        //            dynamic parsed = JsonConvert.DeserializeObject(respStr);
        //            string aiText = parsed.choices[0].message.content.ToString().Trim();

        //            //Remove markdown ```json or ``` wrappers
        //            if (aiText.StartsWith("```"))
        //            {
        //                aiText = aiText.Replace("```json", "")
        //                               .Replace("```", "")
        //                               .Trim();
        //            }

        //            //Remove unwanted prefix/suffix text
        //            int firstBracket = aiText.IndexOf('[');
        //            int lastBracket = aiText.LastIndexOf(']');

        //            if (firstBracket >= 0 && lastBracket > firstBracket)
        //            {
        //                aiText = aiText.Substring(firstBracket, lastBracket - firstBracket + 1);
        //            }

        //            // 3️⃣ Finally parse JSON
        //            return JsonConvert.DeserializeObject<List<QuizQuestionDto>>(aiText);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new List<QuizQuestionDto>
        //{
        //    new QuizQuestionDto { Question = "Error: " + ex.Message }
        //};
        //    }
        //}


        //#endregion GenerateQuizFromOpenAI




        #region GenerateQuizFromGemini

        private async Task<List<QuizQuestionDto>> GenerateQuizFromGeminiAI(string textContent)
        {
            try
            {
                string apiKey = System.Configuration.ConfigurationManager
                                .AppSettings["Gemini_ApiKey"];

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

                    var requestBody = new
                    {
                        contents = new[]
                        {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text =
                                "Return ONLY VALID JSON. No explanation. No markdown.\n\n" +
                                "Generate EXACTLY 5 MCQs ONLY in this JSON format:\n" +
                                "[{\"Question\":\"Q1?\",\"OptionA\":\"A\",\"OptionB\":\"B\",\"OptionC\":\"C\",\"OptionD\":\"D\",\"Answer\":\"A\"}]\n\n" +
                                "Content:\n" + textContent
                            }
                        }
                    }
                }
                    };

                    var json = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    string url =
                        "https://generativelanguage.googleapis.com/v1beta/models/" +
                        "gemini-2.5-flash:generateContent";

                    var response = await client.PostAsync(url, content);
                    var respStr = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                        return new List<QuizQuestionDto>
                {
                    new QuizQuestionDto { Question = "Gemini API error: " + respStr }
                };

                    dynamic parsed = JsonConvert.DeserializeObject(respStr);
                    string aiText = parsed.candidates[0].content.parts[0].text.ToString().Trim();

                    int firstBracket = aiText.IndexOf('[');
                    int lastBracket = aiText.LastIndexOf(']');

                    if (firstBracket >= 0 && lastBracket > firstBracket)
                        aiText = aiText.Substring(firstBracket, lastBracket - firstBracket + 1);

                    return JsonConvert.DeserializeObject<List<QuizQuestionDto>>(aiText);
                }
            }
            catch (Exception ex)
            {
                return new List<QuizQuestionDto>
        {
            new QuizQuestionDto { Question = "Error: " + ex.Message }
        };
            }
        }

        #endregion GenerateQuizFromGemini





        #region SaveQuiz
        [HttpPost]
        public ActionResult SaveQuiz(SaveQuizRequest model)
        {
            try
            {
                
                if (Session["UserId"] == null || Session["RoleId"].Equals("3"))
                {
                    return RedirectToAction("Login", "Account");
                }

                int instructorId = Convert.ToInt32(Session["UserId"]);
                QuizBAL bal = new QuizBAL();

                //To Insert main quiz
                int quizId = bal.cls_InsertQuiz(model.CourseId, model.Title, model.Description, instructorId, model.DurationInMinutes);

                // To Insert all questions With Return QuizId
                foreach (var q in model.Questions)
                {
                    bal.cls_InsertQuizQuestion(
                        quizId,
                        q.QuestionText,
                        q.OptionA,
                        q.OptionB,
                        q.OptionC,
                        q.OptionD,
                        q.CorrectOption
                    );
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region EditQuiz
        public ActionResult EditQuiz()
        {
            return View();
        }
        #endregion

        #region SubmitQuiz
        public ActionResult SubmitQuiz()
        {
            return View();
        }
        #endregion

        
    }
}