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
            else if (FileType == ".jpg" || FileType == ".jpeg" || FileType == ".png")
            {
                // 🔹 For image-based text, use OCR (optional: Tesseract)
                extractedText = "This is a placeholder text extracted from image.";
            }
            else if (FileType == ".mp4")
            {
                // 🔹 For videos, you’d need transcript extraction — optional for now
                extractedText = "This is a placeholder transcript from video.";
            }
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
                var quizQuestions = await GenerateQuizFromAI(extractedText);

                // Step 3: Return result
                return Json(new { success = true, message = "Quiz generated successfully!", quiz = quizQuestions });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
        #endregion

        #region GenerateQuizFromGemini
        private async Task<List<object>> GenerateQuizFromAI(string textContent)
        {
            try
            {
                string apiKey = "AIzaSyB58YQtxDbafU7QR4sUn_qrfOzSs9EqyUE";
                string model = "gemini-1.5-flash";
                string apiUrl = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";

                using (var client = new HttpClient())
                {
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
                                text = $"Generate 5 multiple-choice questions with 4 options and correct answers based on this content:\n\n{textContent}\n\n" +
                                       "Return the output in pure JSON format as an array like this:\n" +
                                       "[{ \"question\": \"...\", \"options\": [\"A\",\"B\",\"C\",\"D\"], \"answer\": \"...\" }]"
                            }
                        }
                    }
                }
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(apiUrl, content);
                    var result = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        return new List<object>
                {
                    new { QuestionText = $"API Error: {response.StatusCode}, Response: {result}" }
                };
                    }

                    dynamic jsonResponse = JsonConvert.DeserializeObject(result);
                    string aiOutput = jsonResponse?.candidates?[0]?.content?.parts?[0]?.text?.ToString();

                    if (string.IsNullOrWhiteSpace(aiOutput))
                        return new List<object> { new { QuestionText = "No quiz generated. Try again." } };

                    // Clean JSON output if Gemini adds formatting or extra text
                    aiOutput = aiOutput.Trim();
                    if (aiOutput.StartsWith("```")) aiOutput = aiOutput.Trim('`').Replace("json", "").Trim();

                    var quizList = JsonConvert.DeserializeObject<List<object>>(aiOutput);
                    return quizList ?? new List<object> { new { QuestionText = "Error parsing quiz." } };
                }
            }
            catch (Exception ex)
            {
                return new List<object>
        {
            new { QuestionText = "Error generating quiz: " + ex.Message }
        };
            }
        }
        #endregion


        //#region GenerateQuizFromAI

        //private async Task<List<object>> GenerateQuizFromAI(string textContent)
        //{
        //    string apiKey = "YOUR_OPENAI_API_KEY"; // 🔒 store securely
        //    string prompt = $"Generate 5 multiple-choice questions with 4 options each and correct answers based on this content:\n{textContent}";

        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        //        var requestData = new
        //        {
        //            model = "gpt-3.5-turbo",
        //            messages = new[]
        //            {
        //                new { role = "system", content = "You are a quiz generation assistant." },
        //                new { role = "user", content = prompt }
        //            },
        //            temperature = 0.7
        //        };

        //        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
        //        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

        //        string result = await response.Content.ReadAsStringAsync();

        //        dynamic json = JsonConvert.DeserializeObject(result);
        //        string aiOutput = json.choices[0].message.content;

        //        // 🔹 Parse AI output (optional: more structured parsing)
        //        List<object> quizList = new List<object>
        //        {
        //            new { QuestionText = aiOutput }
        //        };

        //        return quizList;
        //    }
        //}


        //#endregion GenerateQuizFromAI



        #region SaveQuiz
        public ActionResult SaveQuiz()
        {
            return View();
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