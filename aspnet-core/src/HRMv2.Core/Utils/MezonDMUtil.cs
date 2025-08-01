using HRMv2.Manager.Notifications.SendMezonDM.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HRMv2.Utils
{
    public class MezonDMUtil
    {
        public static List<MK_Link> GetListIndexOfLinks(string message)
        {
            string pattern = @"https?://[^\s]+";
            List<MK_Link> links = new List<MK_Link>();
            MatchCollection matches = Regex.Matches(message, pattern);
            foreach (Match match in matches)
            {
                links.Add(new MK_Link
                {
                    Type = "lk",
                    Start = match.Index,
                    End = match.Index + match.Length,
                });
            }
            return links;
        }
    }
}
