using ClosedXML.Excel;
using DemoMVC.Models;
using DemoMVC.Service;
using DemoMVC.WebUi.Helper;
using DemoMVC.WebUi.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    [Authorize]
    public class QuestionTypeController : BaseController
    {
        private readonly QuestionTypeService _questionTypeService;
        private readonly MessageService _messageService;
        public QuestionTypeController()
        {
            _questionTypeService = new QuestionTypeService();
            _messageService = new MessageService();
        }
        // GET: Subject
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTIONTYPE.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            return View();
        }

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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTIONTYPE.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;

            QuestionTypeModel model = new QuestionTypeModel();
            if (id.HasValue)
            {
                var questionTypeData = _questionTypeService.GetQuestionTypeById(id.Value);
                model.Id = id.Value;
                model.QuestionTypeCode = questionTypeData.QuestionTypeCode;
                model.Name = questionTypeData.QuestionTypeName;

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(QuestionTypeModel model)
        {
            string actionPermission = "";
            if (model.Id == 0)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if (model.Id > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTIONTYPE.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;
            if (ModelState.IsValid)
            {
                SaveAndUpdateQuestionType(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        public QuestionTypeModel SaveAndUpdateQuestionType(QuestionTypeModel model)
        {
            QuestionType obj = new QuestionType();
            if (model.Id > 0)
            {
                obj = _questionTypeService.GetQuestionTypeById(model.Id);
            }
            int userId = SessionHelper.UserId;
            obj.QuestionTypeId = model.Id;
            obj.QuestionTypeName = model.Name;

            if (obj.QuestionTypeId == 0)
            {
                obj.CreatedBy = userId;
                obj.CreatedOn = DateTime.UtcNow;
                obj.QuestionTypeCode = model.QuestionTypeCode;
                model.Id = _questionTypeService.CreateQuestionType(obj);
            }
            else
            {
                obj.UpdatedBy = userId;
                obj.UpdatedOn = DateTime.UtcNow;
                _questionTypeService.UpdateQuestionType(obj);
            }
            return model;

        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request,string searchTerm)
        {
            var data = _questionTypeService.GetAllQuestionTypesGridModels();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                data = data.Where(q =>
                    (q.Name != null && q.Name.ToLower().Contains(searchTerm)) ||
                    (q.QuestionTypeCode != null && q.QuestionTypeCode.ToLower().Contains(searchTerm))
                );
            }
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDuplicateQuestionTypeCode(string QTypeCode, int Id)
        {
            var getQuestionTypeDetails = _questionTypeService.CheckDuplicateQuestionTypeCode(QTypeCode);
            if (Id > 0)
            {
                getQuestionTypeDetails = getQuestionTypeDetails.Where(a => a.QuestionTypeId != Id).ToList();
            }
            if (getQuestionTypeDetails.Count() > 0)
            {
                var message = _messageService.GetMessageByCode(DemoMVC.Helper.Constants.MessageCode.CODEEXIST);
                return Json(message, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(true);
            }
        }

        public ActionResult ExportQuestionType()
        {
            DataTable dt = _questionTypeService.GetQuestionType();

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Question Type");

                // Add DataTable
                ws.Cell(2, 1).InsertTable(dt);

                // Title Row
                ws.Cell("A1").Value = "Question Type Report";

                CommonUtility.DesignExcelExport(ws, dt.Columns.Count);

                // Export
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "QuestionType.xlsx");
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportExcel(HttpPostedFileBase ExcelFile)
        {
            if (ExcelFile != null && ExcelFile.ContentType.Contains("spreadsheet"))
            {
                try
                {
                    var allQuestionTypes = _questionTypeService.GetAllQuestionTypes().Select(x=>x.QuestionTypeCode);
                    using (var stream = ExcelFile.InputStream) 
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1); 
                        var rows = worksheet.RangeUsed().RowsUsed().Skip(2);

                        var questionTypes = new List<QuestionType>();

                        foreach (var row in rows)
                        {
                            if (!allQuestionTypes.Contains(row.Cell(2).GetValue<string>().ToUpper()))
                            {
                                var questionType = new QuestionType
                                {
                                    QuestionTypeName = row.Cell(1).GetValue<string>(), 
                                    QuestionTypeCode = row.Cell(2).GetValue<string>().ToUpper(),
                                    CreatedOn = DateTime.UtcNow,
                                    CreatedBy = SessionHelper.UserId
                                };
                                _questionTypeService.CreateQuestionType(questionType);
                            }
                        }

                        TempData["Success"] = "Excel Imported Successfully!";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error occurred while importing Excel: " + ex.Message;
                }
            }
            else
            {
                TempData["Error"] = "Please upload a valid Excel file.";
            }

            return RedirectToAction("Index");
        }
    }
}