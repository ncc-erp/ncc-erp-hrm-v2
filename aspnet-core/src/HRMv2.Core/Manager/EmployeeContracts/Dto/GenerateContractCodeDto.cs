using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.EmployeeContracts.Dto
{
    public class GenerateContractCodeDto
    {
        public long EmployeeId { get; set; }
        public UserType UserType { get; set; }
        public long JobPositionId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string ContractTypeName
        {
            get
            {
                switch (UserType)
                {
                    case UserType.ProbationaryStaff:
                        return "HĐTV-NCC";
                    case UserType.Staff:
                        return "HĐLĐ-NCC";

                    case UserType.Collaborators:
                        return "HĐCTV-NCC";
                    case UserType.Internship:
                        return "HĐĐT-NCC";
                        default: return UserType.ToString();

                }

            }
        }
    }
}
