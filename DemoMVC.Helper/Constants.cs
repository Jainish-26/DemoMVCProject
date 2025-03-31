namespace DemoMVC.Helper
{
    public static class Constants
    {
        public static class RoleCode
        {
            public const string SADMIN = "SADMIN";
            public const string ADMIN = "ADMIN";
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

        public static class RequirementStatusCode
        {
            public const string OPEN = "OPEN";
            public const string CLOSE = "CLOSE";
        }
        public static class OTPHelperCode
        {
            public const string OTPEXPIRYMINUTES = "OTPEXPIRYMINUTES";
        }

        public static class QuestionDifficultyLevel
        {
            public const string EASY = "EASY";
            public const string MEDIUM = "MEDIUM";
            public const string HARD = "HARD";
        }

        public static class ExamStatus
        {
            public const string DRAFT = "DRAFT";
            public const string PUBLISHED = "PUBLISHED";
        }
        public static class UserExamStatus
        {
            public const string PENDING = "PENDING";
            public const string ONGOING = "ONGOING";
            public const string COMPLETED = "COMPLETED";
        }
        public static class ResultStatus
        {
            public const string PENDING = "PENDING";
            public const string PASS = "PASS";
            public const string FAIL = "FAIL";
            public const string INREVIEW = "INREVIEW";
        }
    }
}
