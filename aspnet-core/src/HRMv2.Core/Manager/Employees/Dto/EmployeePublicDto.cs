using HRMv2.Utils;
using System.Text.Json.Serialization;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Employees.Dto
{
    public class EmployeePublicDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Avatar => FileUtil.FullFilePath(OriginalAvatar);

        [JsonIgnore]
        public string OriginalAvatar { get; set; }
        public Sex Sex { get; set; }
        public string LevelCode {get; set;}
        public string BranchCode { get; set; }
        public string JobPositionCode { get; set; }
        public UserType UserType { get; set; }
        public EmployeeStatus Status { get; set; }
        public string MezonUserId { get; set; }
    }
}
