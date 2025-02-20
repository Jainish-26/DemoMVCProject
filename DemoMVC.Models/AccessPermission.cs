namespace DemoMVC.Models
{
    public static class AccessPermission
    {
        public static string IsMenu = "IsMenu";
        public static string IsView = "IsView";
        public static string IsAdd = "IsAdd";
        public static string IsEdit = "IsEdit";
        public static string IsDelete = "IsDelete";

        public static bool AllowMenu { get; set; }
        public static bool View { get; set; }
        public static bool Add { get; set; }
        public static bool Edit { get; set; }
        public static bool Delete { get; set; }
    }
}
