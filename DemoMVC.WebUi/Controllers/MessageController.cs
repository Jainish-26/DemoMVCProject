using DemoMVC.Helper;
using DemoMVC.Models;
using DemoMVC.Service;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    [Authorize]
    public class MessageController : BaseController
    {
        string formCode = AuthorizeFormAccess.FormAccessCode.MESSAGE.ToString();
        private readonly MessageService _messageService;
        public MessageController()
        {
            _messageService = new MessageService();
        }
        public ActionResult Index()
        {
            if (!CheckPermission(formCode, AccessPermission.IsView))
                return AccessDenied();

            return View();
        }
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            if (!CheckPermission(formCode, AccessPermission.IsView))
                return AccessDenied();
            var messageData = _messageService.GetAllMessageData();
            return Json(messageData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Message_Create([DataSourceRequest] DataSourceRequest request, Message_Mst msg)
        {
            CustomModelValidation(msg);
            if (msg != null && ModelState.IsValid)
            {
                Message_Mst msgMst = new Message_Mst();
                msgMst.Code = msg.Code.ToUpper();
                msgMst.CreatedBy = SessionHelper.UserId;
                msgMst.CreatedOn = DateTime.UtcNow;
                msgMst.IsActive = msg.IsActive;
                msgMst.Message = msg.Message;
                msgMst.Comment = msg.Comment;
                _messageService.CreateMessage(msgMst);
            }
            return Json(new[] { msg }.ToDataSourceResult(request, ModelState));
        }
        private bool CustomModelValidation(Message_Mst model)
        {
            bool isValid = true;
            var getMessageDetails = _messageService.CheckDuplicateMessageCode(model.Code);
            if (model.Id > 0)
            {
                getMessageDetails = getMessageDetails.Where(a => a.Id != model.Id).ToList();
            }
            if (getMessageDetails.Count() > 0)
            {
                var message = _messageService.GetMessageByCode(Constants.MessageCode.CODEEXIST);
                ModelState.AddModelError("Code", message);
                isValid = false;
            }
            return isValid;
        }
        [HttpPost]
        public ActionResult Message_Update([DataSourceRequest] DataSourceRequest request, Message_Mst msg)
        {
            CustomModelValidation(msg);
            if (msg != null && ModelState.IsValid)
            {
                var messageDetails = _messageService.GetMessageDataById(msg.Id);
                messageDetails.Code = msg.Code.ToUpper();
                messageDetails.ModifiedBy = SessionHelper.UserId;
                messageDetails.ModifiedOn = DateTime.UtcNow;
                messageDetails.IsActive = msg.IsActive;
                messageDetails.Message = msg.Message;
                messageDetails.Comment = msg.Comment;
                _messageService.UpdateMessage(messageDetails);
            }
            return Json(new[] { msg }.ToDataSourceResult(request, ModelState));
        }
    }
}