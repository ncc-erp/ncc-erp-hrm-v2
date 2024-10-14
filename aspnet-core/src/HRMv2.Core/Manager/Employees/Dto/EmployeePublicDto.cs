using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Employees.Dto
{
    public class EmployeePublicDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public Sex Sex { get; set; }
        public string LevelCode {get; set;}
        public string BranchCode { get; set; }
        public string JobPositionCode { get; set; }
        public UserType UserType { get; set; }
        public EmployeeStatus Status { get; set; }
    }
}
