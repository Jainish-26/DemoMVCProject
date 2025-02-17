using DemoMVC.Service;

namespace DemoMVC.WebUi.Helper
{
    public static class FormUtility
    {
        public static string GetFormName(string formCode)
        {
            if (!string.IsNullOrEmpty(formCode))
            {
                FormsService _formservice = new FormsService();
                var getform = _formservice.GetFormsByCode(formCode);
                if (getform != null)
                {
                    return getform.Name;
                }
            }
            return "";
        }

        public static string GetParentFormName(string formCode)
        {
            if (!string.IsNullOrEmpty(formCode))
            {
                FormsService _formservice = new FormsService();
                var getform = _formservice.GetFormsByCode(formCode);
                if (getform != null && getform.ParentFormId != null)
                {
                    var getParentform = _formservice.GetFormsById((int)getform.ParentFormId);
                    if (getParentform != null)
                    {
                        return getParentform.Name;
                    }
                }
            }
            return "";
        }

        public static string getFormNavigation(string formCode)
        {
            if (!string.IsNullOrEmpty(formCode))
            {
                FormsService _formservice = new FormsService();
                var getform = _formservice.GetFormsByCode(formCode);
                if (getform != null)
                {
                    return getform.NavigateURL;
                }
            }
            return "";
        }
    }
}