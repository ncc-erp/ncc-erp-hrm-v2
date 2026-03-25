using static HRMv2.Constants.Enum.HRMEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Home.Dtos
{
    public class HomepageEmployeePopupInputDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long? BranchId { get; set; }
        public UserType? UserType { get; set; }
    }
}
