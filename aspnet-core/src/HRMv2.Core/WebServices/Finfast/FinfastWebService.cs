using Abp.Dependency;
using Abp.Runtime.Session;
using AutoMapper.Configuration;
using HRMv2.WebServices.Dto;
using HRMv2.WebServices.Finfast.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.WebServices.Finfast
{
    public class FinfastWebService : BaseWebService
    {
        public FinfastWebService(HttpClient httpClient,
            IAbpSession abpSession,
            IIocResolver iocResovler
            ) : base(httpClient, abpSession, iocResovler)
        {
        }

        public void CreatOutcomeRequest(InputCreateOucomeRequestDto input)
        {
            Post("api/services/app/Hrmv2/CreateOucomingEntryByHRM", input);
        }

        public void CreateBankAndBankAccountByHRM(InputCreateBankAccountDto input)
        {
            Post("api/services/app/Hrmv2/CreateBankAndBankAccountByHRM", input);

        }

        public List<string> ValidCreateFinfastOutcomeEntry(InputValidCreateFinfastOucome input)
        {
            var response =  PostAsync<AbpResponseResult<List<string>>> ("api/services/app/Hrmv2/ValidPayrollFromHrm", input).Result;
            return response?.Result;
        }

        public GetResultConnectDto CheckConnectToFinfast()
        {
            var res = GetAsync<AbpResponseResult<GetResultConnectDto>>("api/services/app/Public/CheckConnect").Result;
            if (res == null)
            {
                return new GetResultConnectDto
                {
                    IsConnected = false,
                    Message = "Can not connect to Finfast"
                };
            }
            if (res.Error != null)
            {
                return new GetResultConnectDto
                {
                    IsConnected = false,
                    Message = res.Error.Message
                };
            }
            return res.Result;

        }
    }
}
