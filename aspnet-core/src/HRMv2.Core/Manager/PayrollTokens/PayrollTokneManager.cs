using HRMv2.Entities;
using HRMv2.Manager.PayrollTokens.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
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
            
            var payslipDetailIds = WorkScope.GetAll<PayrollToken>().Select(x => x.ReferenceId).ToList();

            var payrollIds = WorkScope.GetAll<PayslipDetail>().Include(x => x.Payslip)
                .Where(x => payslipDetailIds.Contains(x.Id)).Select(a => a.Payslip.PayrollId).ToList();

            var query = WorkScope.GetAll<Payroll>().Where(x => payrollIds.Contains(x.Id))
                .OrderByDescending(x => x.ApplyMonth)
                .Select(x => new PayrollTokenDto
                {
                    PayrollId = x.Id,
                    ApplyMonth = x.ApplyMonth,
                });    

            return await query.GetGridResult(query, input);
        }

        public async Task<GridResult<PayslipTokenDto>> GetAllPagingPayslip(GridParam input, long payrollId)
        {
            var paySlipIds = await WorkScope.GetAll<Payslip>()
                .Where(x => x.PayrollId == payrollId)
                .Select(x => x.Id)
                .ToListAsync();

            var payayslipDetailIds = await WorkScope.GetAll<PayslipDetail>()
                .Where(x => paySlipIds.Contains(x.PayslipId))
                .Select(x => x.Id)
                .ToListAsync();

            var employeeEmail = WorkScope.GetAll<Employee>()
                .Select(x => new { x.Id, x.Email });

            var query = from pt in WorkScope.GetAll<PayrollToken>()
                        join emp in employeeEmail on pt.EmployeeId equals emp.Id 
                        where payayslipDetailIds.Contains(pt.ReferenceId)
                        select new PayslipTokenDto
                        {
                            Id = pt.Id,
                            EmailAddress = emp.Email,
                            Note = pt.Note,
                            Amount = pt.Amount,
                            Status = pt.Status,
                        };

            return await query.GetGridResult(query, input); 
        }





        public async Task SendToken(long payslipId)
        {
            var paySlipToken = WorkScope.GetAll<PayrollToken>().FirstOrDefault(x => x.Id == payslipId);
            paySlipToken.Status = Constants.Enum.HRMEnum.StatusSendToken.Done;
            WorkScope.UpdateAsync(paySlipToken);
            CurrentUnitOfWork.SaveChanges();
        }

        public async Task DeletePayrollToken(long payrollId)
        {
            var payslipIds = WorkScope.GetAll<Payslip>().Where(x => x.PayrollId == payrollId).Select(x => x.Id).ToList();
            var payslipDetailIds = WorkScope.GetAll<PayslipDetail>()
                .Where(x => payslipIds.Contains(x.PayslipId))
                .Select(x => x.Id)
               .ToList();

            var payrollTokens = WorkScope.GetAll<PayrollToken>()
                .Where(x => payslipDetailIds.Contains(x.ReferenceId))
                .ToList();

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
