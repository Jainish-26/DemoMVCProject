using ClosedXML.Excel;
using DemoMVC.Helper;
using DemoMVC.Models;
using DemoMVC.Service;
using DemoMVC.WebUi.Helper;
using DemoMVC.WebUi.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static DemoMVC.Helper.Constants;

namespace DemoMVC.WebUi.Controllers
{
    public class QuestionController : BaseController
    {
        public readonly QuestionService _questionService;
        public readonly AnswerService _answerService;
        public readonly QuestionTypeService _questionTypeService;
        public readonly SubjectService _subjectService;
        public readonly CommonLookupService _commonLookupService;
        public readonly QuestionMediaService _questionMediaService;

        public QuestionController()
        {
            _answerService = new AnswerService();
            _questionService = new QuestionService();
            _questionTypeService = new QuestionTypeService();
            _subjectService = new SubjectService();
            _commonLookupService = new CommonLookupService();
            _questionMediaService = new QuestionMediaService();
        }
        // GET: Question
        public ActionResult Index(int? ExamId)
        {
            //if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), AccessPermission.IsView))
            //{
            //    return RedirectToAction("AccessDenied", "Base");
            //}
            ViewBag.ExamId = ExamId;
            return View();
        }

        [HttpGet]
        public ActionResult Create(int? id)
        {
            string actionPermission = "";
            if (id == null)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if ((id ?? 0) > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            int userId = SessionHelper.UserId;
            QuestionAndAnswerModel model = new QuestionAndAnswerModel();
            //List<QuestionMedia> imageList = new List<QuestionMedia>();

            if (id.HasValue)
            {
                var que = _questionService.GetById(id.Value);

                if (que != null)
                {
                    model.QuestionId = id.Value;
                    model.QuestionText = que.QuestionText;
                    model.Marks = que.Marks;
                    model.IsActive = que.IsActive;
                    model.Difficulty = que.Difficulty;
                    model.SubjectId = que.SubjectId;
                    model.QuestionTypeId = que.QuestionTypeId;
                    model.Answers = _answerService.GetByQuestionId(que.QuestionId).Select(

                        ans => new AnswerViewModel
                        {
                            QuestionId = ans.QuestionId,
                            AnswerId = ans.AnswerId,
                            AnswerText = ans.AnswerText,
                            IsCorrect = ans.IsCorrect,
                        }).ToList();

                    var media = _questionMediaService.GetMediaByQuestionId(que.QuestionId);

                    if (media.Any())
                    {
                        model.QuestionImages = media;
                    }
                }
            }
            BindLevel(ref model);
            BindQuestionType(ref model);
            BindSubject(ref model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(QuestionAndAnswerModel model, List<HttpPostedFileBase> newFiles, List<string> ExistingImages)
        {
            string actionPermission = "";
            if (model.QuestionId == 0)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if (model.QuestionId > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            if (ModelState.IsValid && model.Answers.Count != 0)
            {
                int CorrectCnt = 0;
                int OptionCnt = 0;
                foreach (var ans in model.Answers)
                {
                    if (ans.AnswerText != null && ans.IsCorrect == true)
                    {
                        CorrectCnt++;
                        OptionCnt++;
                    }
                    else if (ans.AnswerText != null)
                    {
                        OptionCnt++;
                    }
                }

                if (CorrectCnt > 0)
                {
                    if ((model.QuestionTypeId == 1 || model.QuestionTypeId == 2))
                    {
                        if (OptionCnt >= 2)
                        {
                            SaveAndUpdateQuestion(model, newFiles, ExistingImages);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("Answers", "Minimum 2 options Required");
                            BindSubject(ref model);
                            BindLevel(ref model);
                            BindQuestionType(ref model);
                            return View(model);
                        }
                    }
                    else
                    {
                        SaveAndUpdateQuestion(model, newFiles, ExistingImages);
                        return RedirectToAction("Index");
                    }
                }
                else
                {

                    BindSubject(ref model);
                    BindLevel(ref model);
                    BindQuestionType(ref model);
                    return View(model);
                }
            }
            else
            {
                BindSubject(ref model);
                BindLevel(ref model);
                BindQuestionType(ref model);
                return View(model);
            }
        }

        public QuestionAndAnswerModel SaveAndUpdateQuestion(QuestionAndAnswerModel model, List<HttpPostedFileBase> newFiles, List<string> ExistingImages)
        {
            int userId = SessionHelper.UserId;
            Questions que = new Questions();
            Answers ans = new Answers();
            List<QuestionMedia> imageList = new List<QuestionMedia>();

            if (model.QuestionId > 0)
            {
                que = _questionService.GetById(model.QuestionId);
            }

            que.QuestionText = model.QuestionText;
            que.Marks = model.Marks;
            que.IsActive = model.IsActive;
            que.Difficulty = model.Difficulty;
            que.SubjectId = model.SubjectId;
            que.QuestionTypeId = model.QuestionTypeId;

            if (model.QuestionId == 0)
            {

                que.CreatedBy = userId;
                que.CreatedOn = DateTime.UtcNow;
                que.QuestionId = _questionService.CreateQuestion(que);

                if (newFiles.Any())
                {
                    foreach (var i in newFiles)
                    {
                        if (i != null)
                        {
                            var media = new QuestionMedia
                            {
                                QuestionId = que.QuestionId,
                                MediaName = DateTime.Now.ToString("yyMMddHHmmssfff"),
                                MediaType = Path.GetExtension(i.FileName)
                            };
                            var QueMedia = media.MediaName + media.MediaType;
                            i.SaveAs(Server.MapPath("~/content/QuestionImage/") + QueMedia);
                            imageList.Add(media);
                        }
                    }
                    _questionMediaService.AddMedia(imageList);
                }

                if (model.Answers == null || !model.Answers.Any(a => !string.IsNullOrWhiteSpace(a.AnswerText)))
                {
                    ModelState.AddModelError("AnswerText", "At least one answer must be provided.");
                    return model;
                }

                int correctCount = model.Answers.Count(a => a.IsCorrect);
                if (correctCount == 0)
                {
                    HttpNotFound();
                    return model;
                }

                foreach (var answer in model.Answers)
                {
                    Answers newAnswer = new Answers
                    {
                        AnswerText = answer.AnswerText,
                        IsCorrect = answer.IsCorrect,
                        QuestionId = que.QuestionId,
                        CreatedBy = userId,
                        CreatedOn = DateTime.UtcNow
                    };
                    newAnswer.AnswerId = _answerService.CreateAnswer(newAnswer);
                }
            }
            else
            {
                que.UpdatedBy = userId;
                que.UpdatedOn = DateTime.UtcNow;
                _questionService.UpdateQuestions(que);
                List<QuestionMedia> addImages = new List<QuestionMedia>();
                var existingImages = _questionMediaService.GetMediaByQuestionId(que.QuestionId);

                var existingIds = existingImages.Select(x => x.Id);

                var submittedIds = model.QuestionImages?.Select(x => x.Id).ToList() ?? new List<int>();

                List<int> deleteIds = new List<int>();

                foreach (var img in existingImages)
                {
                    if (!submittedIds.Contains(img.Id))
                    {
                        deleteIds.Add(img.Id);
                    }
                }

                _questionMediaService.RemoveExistingImages(deleteIds);

                if (newFiles.Any())
                {
                    foreach (var i in newFiles)
                    {
                        if (i != null)
                        {
                            var media = new QuestionMedia
                            {
                                QuestionId = que.QuestionId,
                                MediaName = DateTime.Now.ToString("yyMMddHHmmssfff"),
                                MediaType = Path.GetExtension(i.FileName)
                            };
                            var QueMedia = media.MediaName + media.MediaType;
                            i.SaveAs(Server.MapPath("~/content/QuestionImage/") + QueMedia);
                            addImages.Add(media);
                        }
                    }
                    _questionMediaService.AddMedia(addImages);
                }
                var existingAnswers = _answerService.GetByQuestionId(que.QuestionId);

                var answersToKeep = new List<int>();

                foreach (var answer in model.Answers)
                {
                    if (answer.AnswerId > 0)
                    {
                        var existingAnswer = existingAnswers.FirstOrDefault(a => a.AnswerId == answer.AnswerId);
                        if (existingAnswer != null)
                        {
                            if (existingAnswer.AnswerText != answer.AnswerText || existingAnswer.IsCorrect != answer.IsCorrect)
                            {
                                existingAnswer.AnswerText = answer.AnswerText;
                                existingAnswer.IsCorrect = answer.IsCorrect;
                                existingAnswer.UpdatedBy = userId;
                                existingAnswer.UpdatedOn = DateTime.UtcNow;
                                _answerService.UpdateAnswers(existingAnswer);
                            }
                            answersToKeep.Add(existingAnswer.AnswerId);
                        }
                    }
                    else
                    {
                        var matchingAnswer = existingAnswers.FirstOrDefault(a =>
                            a.AnswerText == answer.AnswerText &&
                            !answersToKeep.Contains(a.AnswerId));
                        if (matchingAnswer != null)
                        {
                            if (matchingAnswer.IsCorrect != answer.IsCorrect)
                            {
                                matchingAnswer.IsCorrect = answer.IsCorrect;
                                matchingAnswer.UpdatedBy = userId;
                                matchingAnswer.UpdatedOn = DateTime.UtcNow;
                                _answerService.UpdateAnswers(matchingAnswer);
                            }
                            answersToKeep.Add(matchingAnswer.AnswerId);
                        }
                        else
                        {
                            Answers newAnswer = new Answers
                            {
                                AnswerText = answer.AnswerText,
                                IsCorrect = answer.IsCorrect,
                                QuestionId = que.QuestionId,
                                CreatedBy = userId,
                                CreatedOn = DateTime.UtcNow
                            };
                            var newAnswerId = _answerService.CreateAnswer(newAnswer);
                            answersToKeep.Add(newAnswerId);
                        }
                    }
                }

                foreach (var existingAnswer in existingAnswers)
                {
                    if (!answersToKeep.Contains(existingAnswer.AnswerId))
                    {
                        _answerService.DeleteAnswer(existingAnswer.AnswerId);
                    }
                }
            }

            return model;
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request, string searchTerm)
        {
            var data = _questionService.GetAllQuestionsGridModel();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                data = data
                    .Where(x =>
                        (x.Subject != null && x.Subject.ToLower().Contains(searchTerm.ToLower())) ||
                        (x.Type != null && x.Type.ToLower().Contains(searchTerm.ToLower())) ||
                        (x.QuestionText != null && x.QuestionText.ToLower().Contains(searchTerm.ToLower())) ||
                        (x.Difficulty != null && x.Difficulty.ToLower().Contains(searchTerm.ToLower())) ||
                        (x.Marks != 0 && x.Marks.ToString().Contains(searchTerm))
                    )
                    .AsQueryable();
            }
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public QuestionAndAnswerModel BindQuestionType(ref QuestionAndAnswerModel questionAndAnswerModel)
        {
            var getQuestionTypes = _questionTypeService.GetAllQuestionTypes();
            questionAndAnswerModel._questionTypeList.Add(new SelectListItem() { Text = " Question Type     ", Value = "" });
            foreach (var type in getQuestionTypes)
            {
                questionAndAnswerModel._questionTypeList.Add(new SelectListItem() { Text = type.QuestionTypeName.Trim(), Value = type.QuestionTypeId.ToString() });
            }

            return questionAndAnswerModel;
        }
        public QuestionAndAnswerModel BindSubject(ref QuestionAndAnswerModel questionAndAnswerModel)
        {
            var getSubject = _subjectService.GetAllSubjects();
            questionAndAnswerModel._subjectList.Add(new SelectListItem() { Text = " Subject", Value = "" });
            foreach (var type in getSubject)
            {
                questionAndAnswerModel._subjectList.Add(new SelectListItem() { Text = type.SubjectName.Trim(), Value = type.SubjectId.ToString() });
            }

            return questionAndAnswerModel;
        }

        private QuestionAndAnswerModel BindLevel(ref QuestionAndAnswerModel questionAndAnswerModel)
        {
            var data = _commonLookupService.GetLookupByType(LookupType.QuestionDifficultyLevel);
            questionAndAnswerModel._difficultyList.Add(new SelectListItem() { Text = " Level     ", Value = "" });
            foreach (var item in data)
            {
                questionAndAnswerModel._difficultyList.Add(new SelectListItem() { Text = item.Name, Value = item.Code });
            }
            return questionAndAnswerModel;
        }

        public ActionResult Delete(int? id)
        {
            string actionPermission = AccessPermission.IsDelete;

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), actionPermission))
            {
                return Json(new { success = false, message = "Access Denied" });
            }

            int userId = SessionHelper.UserId;

            try
            {
                bool ans = _questionService.DeleteQuestion(id.Value);

                if (ans)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Question Not Deleted" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult GetImagePopup(int id)
        {
            var question = _questionService.GetAllQuestionsGridModel().FirstOrDefault(q => q.QuestionId == id);

            if (question.QuestionImage == null)
            {
                return HttpNotFound();
            }

            return PartialView("_ImagePopup", question);
        }

        public ActionResult GetQuestionDetails(int id, string access)
        {
            var question = _questionService.GetById(id);
            var media = _questionMediaService.GetMediaByQuestionId(id);
            List<string> imageList = new List<string>();
            if (media.Any())
            {
                foreach (var i in media)
                {
                    var imageName = i.MediaName + i.MediaType;
                    imageList.Add(imageName);
                }
            }
            ViewBag.Access = access;
            QuestionAndAnswerModel model = new QuestionAndAnswerModel
            {
                QuestionText = question.QuestionText,
                IsActive = question.IsActive,
                Difficulty = question.Difficulty,
                QuestionImages = media,
                QuestionType = question.QuestionType.QuestionTypeName,
                Subject = question.Subject.SubjectName,
                Marks = question.Marks,
                Answers = _answerService.GetByQuestionId(id).Select(ans => new AnswerViewModel
                {
                    AnswerText = ans.AnswerText,
                    IsCorrect = ans.IsCorrect
                }).ToList()
            };

            return PartialView("_QuestionDetailsPopUp", model);
        }

        [HttpPost]
        public JsonResult DeleteQuestionImage(int questionId)
        {
            string actionPermission = AccessPermission.IsDelete;

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), actionPermission))
            {
                return Json(new { success = false, message = "Access Denied" });
            }

            int userId = SessionHelper.UserId;

            try
            {
                bool ans = _questionService.DeleteQuestionImage(questionId);


                if (ans)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Image Not Deleted" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public JsonResult CheckQuestionInExam(int QuestionId)
        {
            bool Ans = _questionService.CheckQuestionInExam(QuestionId);
            if (!Ans)
            {
                return Json(new { success = false, message = "Question Already Assigned To Exam" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, questionId = QuestionId }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportQuestionAndAnswerData()
        {
            var dt = _questionService.GetQuestionAndAnswerExportData();

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Questions");
                ws.Cell(2, 1).InsertTable(dt);

                // Design
                CommonUtility.DesignExcelExport(ws, dt.Columns.Count);
                var wrapColIndex = ws.FirstRowUsed().RowBelow().Cells()
                .FirstOrDefault(c => c.Value.ToString() == "AnswerJson")?.Address.ColumnNumber ?? 0;

                if (wrapColIndex > 0)
                {
                    ws.Column(wrapColIndex).Style.Alignment.WrapText = true;
                }

                // Export
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "QuestionExport.xlsx");
                }
            }
        }
        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase excelFile)
        {
            if (excelFile == null || excelFile.ContentLength == 0)
            {
                TempData["Error"] = "Please upload a valid Excel file.";
                return RedirectToAction("Index");
            }
            List<string> invalidQuestions = new List<string>();
            List<Questions> validQuestions = new List<Questions>();
            List<Answers> validAnswers = new List<Answers>();
            List<string> subjects = _subjectService.GetAllSubjects().Select(x => x.SubjectCode).ToList();
            List<string> questionTypes = _questionTypeService.GetAllQuestionTypes().Select(x => x.QuestionTypeCode).ToList();
            List<string> difficultyLevel = _commonLookupService.GetLookupByType("QuestionDifficultyLevel").Select(x => x.Code).ToList();
            int typeId=0;
            int subjectId=0;
            try
            {
                
                using (var workbook = new XLWorkbook(excelFile.InputStream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

                    foreach (var row in rows)
                    {
                        var questionText = row.Cell(1).Value.ToString();
                        int marks;
                        int.TryParse(row.Cell(2).Value.ToString(), out marks);
                        marks = marks > 0 ? marks : 0;
                        var questionType = row.Cell(3).Value.ToString().Replace(" ","");
                        var subject = row.Cell(4).Value.ToString().Replace(" ", "");
                        var difficulty = row.Cell(5).Value.ToString().Replace(" ", "");
                        var answerJson = row.Cell(6).Value.ToString();

                        //DeserializeObject of answerJson
                        var answers = JsonConvert.DeserializeObject<List<AnswerImportModel>>(answerJson);

                        //Get Id Of Type And Subject
                        if (questionTypes.Contains(questionType))
                        {
                             typeId = _questionTypeService.GetQuestionTypetByCode(questionType).QuestionTypeId;
                        }

                        if (subjects.Contains(subject))
                        {
                            subjectId = _subjectService.GetSubjectByCode(subject).SubjectId;
                        }

                        if (questionText != null && questionText != "")
                        {
                            if (marks <= 0)
                            {
                                invalidQuestions.Add($"{row.RowNumber()} has invalid Marks");
                            }
                            else if (!questionTypes.Contains(questionType.ToUpper()) || questionType == null || typeId ==0)
                            {
                                invalidQuestions.Add($"{row.RowNumber()} has invalid Question Type");
                            }
                            else if (!subjects.Contains(subject.ToUpper()) || questionType == null || subjectId ==0)
                            {
                                invalidQuestions.Add($"{row.RowNumber()} has invalid Subject");
                            }
                            else if (!difficultyLevel.Contains(difficulty.ToUpper()) || questionType == null)
                            {
                                invalidQuestions.Add($"{row.RowNumber()} has invalid Question Difficulty");
                            }
                            else if (answers != null)
                            {
                                int CorrectCnt = 0;
                                int OptionCnt = 0;
                                foreach (var ans in answers)
                                {

                                    if (ans.AnswerText != null && ans.IsCorrect == true)
                                    {
                                        CorrectCnt++;
                                        OptionCnt++;
                                    }
                                    else if (ans.AnswerText != null)
                                    {
                                        OptionCnt++;
                                    }
                                }

                                if (CorrectCnt < 1 || OptionCnt < 1)
                                {
                                    invalidQuestions.Add($"{row.RowNumber()} has invalid . 0 correct answer");
                                }
                                else if (OptionCnt < 2 && (typeId == 1 || typeId == 2))
                                {
                                    invalidQuestions.Add($"{row.RowNumber()} has invalid . MCQ Must Have minimun 2 options");
                                }
                                else if (CorrectCnt > 1 && typeId == 1)
                                {
                                    invalidQuestions.Add($"{row.RowNumber()} has invalid Answer . SINGLEMCQ has only 1 correct answer");
                                }
                                else
                                {
                                    var question = new Questions
                                    {
                                        QuestionText = questionText,
                                        Marks = marks,
                                        IsActive = true,
                                        QuestionTypeId = typeId,
                                        SubjectId = subjectId,
                                        Difficulty = difficulty.ToUpper(),
                                        CreatedBy = SessionHelper.UserId,
                                        CreatedOn = DateTime.Now
                                    };

                                    int questionId = _questionService.CreateQuestion(question);
                                    foreach (var ans in answers)
                                    {
                                        var answer = new Answers
                                        {
                                            QuestionId = questionId, // FK link
                                            AnswerText = ans.AnswerText,
                                            IsCorrect = ans.IsCorrect,
                                            CreatedOn = DateTime.Now
                                        };

                                        validAnswers.Add(answer);
                                    }
                                }
                            }
                        }
                        else
                        {
                            invalidQuestions.Add($"{row.RowNumber()} has Null Questions");
                        }
                    }

                    if (invalidQuestions.Any())
                    {
                        TempData["InvalidQuestions"] = invalidQuestions;
                        return RedirectToAction("Index", "Question");
                    }
                    _answerService.AddAnswers(validAnswers);
                    
                    TempData["Success"] = "Excel data imported successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error importing data: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
        public ActionResult DownloadTemplate()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Template");

                // Header titles
                var headers = new[] { "QuestionText", "Marks", "QuestionType", "Subject", "Difficulty", "Answers (JSON Format)" };

                // Set headers
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cell(1, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }

                // Sample data
                worksheet.Cell(2, 1).Value = "What is Java?";
                worksheet.Cell(2, 2).Value = 5;
                worksheet.Cell(2, 3).Value = "SINGLEMCQ";
                worksheet.Cell(2, 4).Value = "JAVA";
                worksheet.Cell(2, 5).Value = "EASY";
                worksheet.Cell(2, 6).Value = "[{\"answerText\":\"Java is a programming language\",\"isCorrect\":true},{\"answerText\":\"Java is a database\",\"isCorrect\":false}]";

                // Style sample row
                for (int i = 1; i <= headers.Length; i++)
                {
                    var cell = worksheet.Cell(2, i);
                    cell.Style.Alignment.WrapText = true;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                }

                worksheet.Columns().AdjustToContents(); // Auto-size columns

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "QuestionTemplate.xlsx");
                }
            }
        }


    }
}