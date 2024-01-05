using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.Charts.ChartDetails.Dto
{
    public class KeyValueDto
    {
        public string Key { get; set; }

        public long Value { get; set; }

        public KeyValueDto(string key, long value)
        {
            Key = key;
            Value = value;
        }

        public KeyValueDto()
        {
        }
    }
}
