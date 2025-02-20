using System;
using System.Collections.Generic;
using System.Web;

namespace DemoMVC.Models
{
    public class SessionHelper : BaseSessionHelper
    {
        public class Constants
        {
            public const string UserId = "CurrentUserId";
            public const string UserName = "CurrentUserName";
            public const string Name = "Name";
            public const string RoleId = "RoleId";
            public const string RoleName = "RoleName";
            public const string RoleCode = "RoleCode";
            public const string DefaultTimeZone = "DefaultTimeZone";
            public const string EmailSignature = "EmailSignature";
            public const string SelectedCustomerId = "SelectedCustomerId";
            public const string SelectedRequirementId = "SelectedRequirementId";
            public const string UserEmailId = "UserEmailId";
            public const string SelectedStatus = "SelectedStatus";
            public const string SelectedReqId = "SelectedReqId";
            public const string SelectedReqIdSendCalendar = "SelectedReqIdSendCalendar";
            public const string SelectedReqIdInterestedCan = "SelectedReqIdInterestedCan";
            public const string SelectedReqIdSendInterview = "SelectedReqIdSendInterview";
            public const string SelectedReqIdUpdateStatus = "SelectedReqIdUpdateStatus";
            public const string SelectedStatusInterestedCan = "SelectedStatusInterestedCan";
            public const string SelectedStatusUpdateStatus = "SelectedStatusUpdateStatus";
            public const string OrganizationIds = "OrganizationIds";
            public const string IsAdmin = "IsAdmin";
        }
        public static bool IsAdmin
        {
            get { return GetSessionValue<bool>(Constants.IsAdmin); }
            set { SetSessionValue(Constants.IsAdmin, value); }
        }
        public static List<int> OrganizationIds
        {
            get { return GetSessionValue<List<int>>(Constants.OrganizationIds); }
            set { SetSessionValue(Constants.OrganizationIds, value); }
        }
        public static int UserId
        {
            get { return GetSessionValue<int>(Constants.UserId); }
            set { SetSessionValue(Constants.UserId, value); }
        }

        public static int RoleId
        {
            get { return GetSessionValue<int>(Constants.RoleId); }
            set { SetSessionValue(Constants.RoleId, value); }
        }
        public static string UserName
        {
            get { return GetSessionValue<string>(Constants.UserName); }
            set { SetSessionValue(Constants.UserName, value); }
        }
        public static string SelectedStatus
        {
            get { return GetSessionValue<string>(Constants.SelectedStatus); }
            set { SetSessionValue(Constants.SelectedStatus, value); }
        }

        public static string Name
        {
            get { return GetSessionValue<string>(Constants.Name); }
            set { SetSessionValue(Constants.Name, value); }
        }

        public static string RoleName
        {
            get { return GetSessionValue<string>(Constants.RoleName); }
            set { SetSessionValue(Constants.RoleName, value); }
        }

        public static string RoleCode
        {
            get { return GetSessionValue<string>(Constants.RoleCode); }
            set { SetSessionValue(Constants.RoleCode, value); }
        }

        public static string DefaultTimeZone
        {
            get { return GetSessionValue<string>(Constants.DefaultTimeZone); }
            set { SetSessionValue(Constants.DefaultTimeZone, value); }
        }
        public static string EmailSignature
        {
            get { return GetSessionValue<string>(Constants.EmailSignature); }
            set { SetSessionValue(Constants.EmailSignature, value); }
        }

        public static string SelectedCustomerId
        {
            get { return GetSessionValue<string>(Constants.SelectedCustomerId); }
            set { SetSessionValue(Constants.SelectedCustomerId, value); }
        }

        public static string SelectedRequirementId
        {
            get { return GetSessionValue<string>(Constants.SelectedRequirementId); }
            set { SetSessionValue(Constants.SelectedRequirementId, value); }
        }

        public static string UserEmailId
        {
            get { return GetSessionValue<string>(Constants.UserEmailId); }
            set { SetSessionValue(Constants.UserEmailId, value); }
        }
        public static string SelectedReqId
        {
            get { return GetSessionValue<string>(Constants.SelectedReqId); }
            set { SetSessionValue(Constants.SelectedReqId, value); }
        }
        public static string SelectedReqIdSendCalendar
        {
            get { return GetSessionValue<string>(Constants.SelectedReqIdSendCalendar); }
            set { SetSessionValue(Constants.SelectedReqIdSendCalendar, value); }
        }
        public static string SelectedReqIdInterestedCan
        {
            get { return GetSessionValue<string>(Constants.SelectedReqIdInterestedCan); }
            set { SetSessionValue(Constants.SelectedReqIdInterestedCan, value); }
        }
        public static string SelectedReqIdSendInterview
        {
            get { return GetSessionValue<string>(Constants.SelectedReqIdSendInterview); }
            set { SetSessionValue(Constants.SelectedReqIdSendInterview, value); }
        }
        public static string SelectedReqIdUpdateStatus
        {
            get { return GetSessionValue<string>(Constants.SelectedReqIdUpdateStatus); }
            set { SetSessionValue(Constants.SelectedReqIdUpdateStatus, value); }
        }
        public static string SelectedStatusInterestedCan
        {
            get { return GetSessionValue<string>(Constants.SelectedStatusInterestedCan); }
            set { SetSessionValue(Constants.SelectedStatusInterestedCan, value); }
        }

        public static string SelectedStatusUpdateStatus
        {
            get { return GetSessionValue<string>(Constants.SelectedStatusUpdateStatus); }
            set { SetSessionValue(Constants.SelectedStatusUpdateStatus, value); }
        }
    }

    public class BaseSessionHelper
    {
        public static T GetSessionValue<T>(string item, bool throwExceptionIfMissing = false)
        {
            if (HttpContext.Current == null)
                throw new ApplicationException("Invalid HTTP Context.");

            if (HttpContext.Current.Session == null)
                throw new ApplicationException("Session Expired.");

            if (HttpContext.Current.Session[item] == null)
            {
                if (throwExceptionIfMissing)
                {
                    throw new ApplicationException(String.Format("{0} is missing", item));
                }
                else
                {
                    return default(T);
                }
            }

            return (T)HttpContext.Current.Session[item];
        }

        public static bool HasSessionValue(string item)
        {
            if (HttpContext.Current == null)
                throw new ApplicationException("Invalid HTTP Context.");

            if (HttpContext.Current.Session == null)
                throw new ApplicationException("Session Expired.");

            return (HttpContext.Current.Session[item] != null);
        }

        public static void SetSessionValue<T>(string item, T value)
        {
            if (!object.Equals(value, default(T)))
                HttpContext.Current.Session[item] = value;
            else
                NullSessionVar(item);
        }

        public static void NullSessionVar(string item)
        {
            HttpContext.Current.Session[item] = null;
        }
    }
}
