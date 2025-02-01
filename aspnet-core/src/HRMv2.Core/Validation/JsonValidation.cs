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
        public static ResultCheckJson IsValidJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new ResultCheckJson() {IsValid = false,ErrorMessage = "Template is empty or null" };
            try
            {
                var obj = JsonConvert.DeserializeObject(input);
                return new ResultCheckJson() { IsValid = true, ErrorMessage = null};
            }
            catch (JsonException ex)
            {
                return new ResultCheckJson() { IsValid = false, ErrorMessage = ex.Message };
            }
        }
    }

    public class ResultCheckJson
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
}
