using HRMv2.Entities;
using HRMv2.Utils;
using NccCore.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.WebServices.Dto
{
    public class CreateOrUpdateUserOtherToolDto
    {
        public Sex Sex { get; set; }
        public UserType Type { get; set; }
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string Surname => CommonUtil.GetSurNameByFullName(FullName);
        public string Name => CommonUtil.GetNameByFullName(FullName);
        public string BranchCode { get; set; }
        public string LevelCode { get; set; }
        public string PositionCode { get; set; }
        public DateTime WorkingStartDate { get; set; }
        public List<string> SkillNames { get; set; }
        public string CurrentAddress { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
    }
}
