using HRMv2.Entities;
using HRMv2.Manager.PayrollTokens.Dto;
using HRMv2.NccCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.PayrollTokens
{
    public class PayrollTokneManager : BaseManager
    {
        public PayrollTokneManager(IWorkScope workScope) : base(workScope)
        {
        }
            public async Task<GridResult<PayrollTokenDto>> GetAllPaging(GridParam input)
            {
            var query = WorkScope.GetAll<PayrollToken>()
                .GroupBy(x => x.Month)
                .Select(x =>new PayrollTokenDto
                {              
                    Month = x.Key,
                });
                return await query.GetGridResult(query, input);
            }

       public async Task<GridResult<PayslipTokenDto>> GetAllPagingPayslip(GridParam input,string month)
        {
            var query = WorkScope.GetAll<PayrollToken>()
                .Where(x => x.Month == month)   
                .Select(x => new PayslipTokenDto
                {
                    Id = x.Id,
                    EmailAddress = x.EmailAddress,
                    Note = x.Note,
                    TokenMezon = x.TokenMezon,
                    Status = x.Status,
                });
               
            return await query.GetGridResult(query, input);
        }

        public async Task SendToken(long payslipId)
        {
            var paySlipToken = WorkScope.GetAll<PayrollToken>().FirstOrDefault(x => x.Id == payslipId);
            paySlipToken.Status = Constants.Enum.HRMEnum.StatusSendToken.Done;
            WorkScope.UpdateAsync(paySlipToken);
            CurrentUnitOfWork.SaveChanges();
        }
        
        public async Task DeletePayrollToken(string month)
        {
            var payrollTokens = WorkScope.GetAll<PayrollToken>()
                .Where(x => x.Month == month).ToList();

            payrollTokens.ForEach(x => x.IsDeleted = true);
            WorkScope.UpdateRangeAsync(payrollTokens);
            CurrentUnitOfWork.SaveChanges();

        }

        public async Task DeletePaySlipToken(long id)
        {
            var payslipToken = WorkScope.GetAll<PayrollToken>().FirstOrDefault(x => x.Id == id);
            payslipToken.IsDeleted = true;
            WorkScope.UpdateAsync(payslipToken);
            CurrentUnitOfWork.SaveChanges();
        }
    }
}
