using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Validation
{
    public class JsonValidation
    {
        public static bool IsValidJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;
            try
            {
                var obj = JsonConvert.DeserializeObject(input);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
