namespace HRMv2.Configuration
{
    public static class AppSettingNames
    {
        public const string UiTheme = "App.UiTheme";
        public const string StorageLocation = "App.StorageLocation";
        public const string EmailHR = "App.EmailHR";
        public const string GoogleClientId = "App.GoogleClientId";
        public const string SecretRegisterCode = "App.SecretRegisterCode";
        public const string SecurityCode = "App.SecurityCode";
        public const string AutoCreateUserToTimesheet = "App.AutoCreateUserTimesheet";
        public const string AutoCreateUserToIMS = "App.AutoCreateUserIMS";
        public const string AutoCreateUserToProject = "App.AutoCreateUserToProject";
        public const string AutoCreateUserToTalent = "App.AutoCreateUserToTalent";
        public const string FinaceUri = "App.FinanceUri";
        public const string FinanceSecretKey = "App.FinanceSecretKey";
        public const string IMSUri = "App.IMSUri";
        public const string IMSSecretKey = "App.IMSSecretKey";
        public const string TimesheetUri = "App.TimesheetUri";
        public const string TimeSheetSecretKey = "App.TimeSheetSecretKey";
        public const string ProjectUri = "App.ProjectUri";
        public const string ProjectSecretKey = "App.SecretKey";
        public const string TalentUri = "App.TalentUri";
        public const string TalentSecretKey = "App.TalentSecretKey";
        public const string EnableNormalLogin = "App.EnableNormalLogin";
        public const string KomuITChannelId = "App.KomuITChannelId";
        public const string PayrollChannelId = "App.PayrollChannelId";
        public const string EnableWorkerAutoUpdateAllEmployeeInfoToOtherToolSetting = "App.EnableWorkerAutoUpdateAllEmployeeInfoToOtherToolSetting";
        public const string AutoUpdateEmployeeInfoToOtherToolAtHour = "App.AutoUpdateEmployeeInfoToOtherToolAtHour";

        public static string FaceUri { get; internal set; }
    }

}