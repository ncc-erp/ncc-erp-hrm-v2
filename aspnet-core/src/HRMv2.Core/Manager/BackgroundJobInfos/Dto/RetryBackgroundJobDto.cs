using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.BackgroundJobInfos.Dto
{
    public class RetryBackgroundJobDto
    {
        public long JobId { get; set; }
        public int TimeToExecute { get; set; }
    }
}
