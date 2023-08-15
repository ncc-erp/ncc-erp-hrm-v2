using HRMv2.Entities;
using HRMv2.Manager.Debts.PaidsManagger.Dto;
using HRMv2.NccCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Debts.PaidsManagger
{
    public class PaidManager : BaseManager
    {
        public PaidManager(IWorkScope workScope) : base(workScope)
        {
        }

        public IQueryable<DebtPaidDto> QueryAllDebt()
        {
            return WorkScope.GetAll<DebtPaid>()
                .Select(x => new DebtPaidDto
                {
                  Id = x.Id,
                  DebtId = x.DebtId,
                  PaymentType = x.PaymentType,
                  UserSalaryId = x.PayslipDetailId,
                  Date = x.Date,
                  Note = x.Note,
                  Money = x.Money,
                  IsAllowEdit = x.PayslipDetailId.HasValue ? false : true,
                  CreationTime = x.CreationTime,
                  CreatorUser = x.CreatorUser.FullName,
                  UpdatedTime = x.LastModificationTime,
                  UpdatedUser = x.LastModifierUser.FullName
                });
        }

        public List<DebtPaidDto> GetAll()
        {
            return QueryAllDebt().ToList();
        }

        public DebtPaidDto Get(long id)
        {
            return QueryAllDebt().Where(x => x.Id == id).FirstOrDefault();
        }

        public List<DebtPaidDto> GetPaidsByDebtId(long debtId)
        {
            return QueryAllDebt()
                .Where(x => x.DebtId == debtId)
                .ToList();
        }

        public async Task<GridResult<DebtPaidDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllDebt();
            return await query.GetGridResult(query, input);
        }

        public async Task<CreatePaidDto> Create(CreatePaidDto input)
        {
            //await ValidCreate(input);
            var entity = ObjectMapper.Map<DebtPaid>(input);
            await WorkScope.InsertAsync(entity);
            return input;
        }

        public async Task<UpdatePaidDto> Update(UpdatePaidDto input)
        {
            //await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<DebtPaid>(input.Id);
            ObjectMapper.Map(input, entity);
            await WorkScope.UpdateAsync(entity);
            return null;
        }

        public async Task<long> Delete(long id)
        {
            //await ValidDelete(id);
            await WorkScope.DeleteAsync<DebtPaid>(id);
            return id;
        }
    }
}
