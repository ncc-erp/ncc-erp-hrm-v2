using HRMv2.Manager.Talent;
using HRMv2.Manager.Talent.Dto;
using HRMv2.NCC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Talent
{
    public class TalentAppService : HRMv2AppServiceBase
    {
        private readonly TalentManager _talentManager;

        public TalentAppService(TalentManager talentManager)
        {
            _talentManager = talentManager;
        }
        [NccAuthentication]
        public void CreateTempEmployeeFromTalent(InputCreateTempEmployeeTalentDto input)
        {
            using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
            {
                _talentManager.CreateTempEmployeeFromTalent(input);
            }
        }

    }
}
