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
        public static List<StartEnd> GetListIndexOfLinks(string message)
        {
            string pattern = @"https?://[^\s]+";
            List<StartEnd> links = new List<StartEnd>();
            MatchCollection matches = Regex.Matches(message, pattern);
            foreach (Match match in matches)
            {
                links.Add(new StartEnd
                {
                    Start = match.Index,
                    End = match.Index + match.Length,
                });
            }
            return links;
        }
    }
}
