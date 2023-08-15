using HRMv2.Debugging;
using System.IO;

namespace HRMv2
{
    public class HRMv2Consts
    {
        public const string LocalizationSourceName = "HRMv2";

        public const string ConnectionStringName = "Default";
     
        public const float WorkingDayValue = 1;
        public const float OpentalkDayValue = 1;
        public const double OpenTalkHourPerday = 4;
        public const float DefaultBonusLeaveDayEachMonth = 1;
        public const int DELAY_SEND_MAIL_SECOND = 1;
        public const int TVIEC_PROBATIONPERCENTAGE  = 85;
        public const int TVIEC_CONTRACT_PERIOD_MONTH  = 2;

        public const bool MultiTenancyEnabled = true;
        public static readonly string templateFolder = Path.Combine("wwwroot", "template");

        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "01067c86e38f4f05ba6d4451e86f4bad";
        
        public const double DebtPaidAllow = 10000;
        public static bool EnableBackgroundJobExecution { get; set; } = true;
        public static string HRM_Uri { get; set; }
    }
}
