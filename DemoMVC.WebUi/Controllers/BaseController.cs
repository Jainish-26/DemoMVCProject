using DemoMVC.Models;
using DemoMVC.Service;
using System.IO;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        private readonly FormRoleMappingService _formService;
        //private readonly CommentService _commentService;
        public BaseController()
        {
            _formService = new FormRoleMappingService();
            //_commentService = new CommentService();
        }
        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }

        public bool CheckPermission(string formCode, string formAction)
        {
            int roleId = SessionHelper.RoleId;

            var checkPermission = _formService.CheckFormAccess(formCode, roleId);
            if (checkPermission != null)
            {
                if (formAction == AccessPermission.IsAdd)
                {
                    return checkPermission.AllowInsert;
                }
                else if (formAction == AccessPermission.IsEdit)
                {
                    return checkPermission.AllowUpdate;
                }
                else if (formAction == AccessPermission.IsDelete)
                {
                    return checkPermission.AllowDelete;
                }
                else if (formAction == AccessPermission.IsMenu)
                {
                    return checkPermission.AllowMenu;
                }
                else if (formAction == AccessPermission.IsView)
                {
                    return checkPermission.AllowView;
                }
                else if (string.IsNullOrWhiteSpace(formAction))
                {
                    if (checkPermission.AllowInsert || checkPermission.AllowUpdate)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public string RenderPartialViewToString(Controller controller, string viewName, object model = null)
        {
            if (model != null)
                controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult;
                viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);

                ViewContext viewContext;
                viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.ToString();
            }
        }
    }
}