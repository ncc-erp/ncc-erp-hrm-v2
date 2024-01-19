using Abp.AutoMapper;
using NccCore.Anotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Categories.Charts.ChartDetails.Dto
{
    [AutoMap(typeof(ChartDetailDto))]
    public class ChartDetailFullDto : ChartDetailSelectionDto
    {
        public long Id { get; set; }

        public long ChartId { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public bool IsActive { get; set; }
    }

    public class ChartDetailSelectionDto
    {
        public List<KeyValueDto> JobPositions { get; set; }
        public List<KeyValueDto> Branches { get; set; }
        public List<KeyValueDto> Levels { get; set; }
        public List<KeyValueDto> Teams { get; set; }
        public List<KeyValueDto> UserTypes { get; set; }
        public List<KeyValueDto> PayslipDetailTypes { get; set; }
        public List<KeyValueDto> Gender { get; set; }
        public List<KeyValueDto> WorkingStatuses { get; set; }

    }
    public class KeyValueDto
    {
        public string Key { get; set; }
        public long Value { get; set; }

        public KeyValueDto()
        {
        }
        public KeyValueDto(string key, long value)
        {
            Key = key;
            Value = value;

        }
    }
}
