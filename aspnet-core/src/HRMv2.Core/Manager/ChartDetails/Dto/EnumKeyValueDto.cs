using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.ChartDetails.Dto
{
    public class EnumKeyValueDto<TEnum>
    {
        public string Key {  get; set; }

        public TEnum Value { get; set; }

        public EnumKeyValueDto(string key, TEnum value) { 
            Key = key;
            Value = value;
        }
    }
}
