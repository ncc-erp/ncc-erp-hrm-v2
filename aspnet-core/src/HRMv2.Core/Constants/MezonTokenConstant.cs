using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Constants
{
    public class MezonTokenConstant
    {
        public static string BotToken { get; set; }
        public static string BotId {  get; set; }
        public static string UrlAuthenticate { get; set; }
    }

    public class MmnConstant
    {
        public static string NodeEndpoint { get; set; }
        public static string ZkProveEndpoint { get; set; }
        public static string HRMTransferType { get; } = "HRMTransfer";
        public static string EphemeralMmnKeyPair { get; set; }

    }

}
