namespace DemoMVC.Helper
{
    public static class Constants
    {
        public static class RoleCode
        {
            public const string SADMIN = "SADMIN";
            public const string ADMIN = "ADMIN";
        }

        public enum EmailCodes
        {
            SENDPROFILETOCANDIDATE,
            SENDMAILTODECIDINGAUTHORITY,
            SENDCALENDARINVITE,
            FORGOTPASSWORD,
            OTPTODECIDINGAUTHORITY,
            SENDINTERVIEWREMINDER,
            OTPTOINTERVIEWER
        }
        public static class ConfigurableValue
        {
            public const double InteviewEndTimeMinute = 45;
        }
        public static class MessageCode
        {
            public const string EMAILSUCCESS = "EMAILSUCCESS";
            public const string EMAILEERROR = "EMAILEERROR";
            public const string OTPERROR = "OTPERROR";
            public const string OTPEXPIRE = "OTPEXPIRE";
            public const string OTPSENTSUCCESS = "OTPSENTSUCCESS";
            public const string OTPSENTERROR = "OTPSENTERROR";
            public const string OTPWRONG = "OTPWRONG";
            public const string PASSWORDRESETLINKEXPIRED = "PASSWORDRESETLINKEXPIRED";
            public const string PASSWORDCHANGESUCCESS = "PASSWORDCHANGESUCCESS";
            public const string PASSWORDERRORMESSAGE = "PASSWORDERRORMESSAGE";
            public const string SAMEPASSWORDMESSAGE = "SAMEPASSWORDMESSAGE";
            public const string OLDPASSWORDINCORRECTMESSAGE = "OLDPASSWORDINCORRECTMESSAGE";
            public const string PASSWORDRESETEMAILSENT = "PASSWORDRESETEMAILSENT";
            public const string CODEEXIST = "CODEEXIST";
            public const string EMAILEXIST = "EMAILEXIST";
            public const string USERNAMEEXIST = "USERNAMEEXIST";
            public const string USERLOGGEDOUT = "USERLOGGEDOUT";
            public const string POSITIONEXIST = "POSITIONEXIST";
        }
        public static class WidgetCode
        {
            public const string ACTIVEREQUIREMENTS = "ACTIVEREQUIREMENTS";
            public const string SHORTLISTED = "SHORTLISTED";
            public const string INTERVIEWING = "INTERVIEWING";
            public const string OFFERED = "OFFERED";
            public const string CURRENTMONTH = "CURRENTMONTH";
            public const string LASTSIXMONTH = "LASTSIXMONTH";
        }
        public static class CommentSourceType
        {
            public const string CANDIDATE = "CANDIDATE";
        }

        public static class ConfigurationStaticValues
        {
            public const string ImageExtension = ".jpg,.jpeg,.png,.svg";
            public const string DocumentExtension = ".txt,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.xps,.pdf";
            public const string VideoExtension = ".webm,.mkv,.flv,.wmv,.mp4,.mpg";
            public const string ThumbSize = "120,80";
        }

        public enum FileType
        {
            Image = 1,
            Video = 2,
            Document = 3
        }
        public static class CandidateStatusCode
        {
            public const string DRAFT = "DRAFT";
            public const string INTERESTED = "INTERESTED";
            public const string NOREPLY = "NOREPLY";
            public const string EMAILED = "EMAILED";
            public const string CALLBACKLATER = "CALLBACKLATER";
        }
        public static class HiringStatusCode
        {
            public const string INTERESTED = "INTERESTED";
            public const string READYFORINTERVIEW = "READYFORINTERVIEW";
            public const string SENTFORINITIALREVIEW = "SENTFORINITIALREVIEW";
            public const string INITIALREVIEWAPPROVED = "INITIALREVIEWAPPROVED";
            public const string INITIALREVIEWREJECTED = "INITIALREVIEWREJECTED";
            public const string INTERVIEWSCHEDULED = "INTERVIEWSCHEDULED";
            public const string SENTCALENDARINVITE = "SENTCALENDARINVITE";
            public const string SENTINTERVIEWREMINDER = "SENTINTERVIEWREMINDER";
            public const string INT1 = "INT1";
            public const string INT2 = "INT2";
            public const string FININT = "FININT";
            public const string INTERVIEWED = "INTERVIEWED";
            public const string OFFERED = "OFFERED";
            public const string JOINED = "JOINED";
            public const string ONNOTICEPERIOD = "ONNOTICEPERIOD";
            public const string ONHOLD = "ONHOLD";
            public const string REJECTED = "REJECTED";

        }
        public static class RequirementStatusCode
        {
            public const string OPEN = "OPEN";
            public const string CLOSE = "CLOSE";
        }
        public static class OTPHelperCode
        {
            public const string OTPEXPIRYMINUTES = "OTPEXPIRYMINUTES";
        }
    }
}
