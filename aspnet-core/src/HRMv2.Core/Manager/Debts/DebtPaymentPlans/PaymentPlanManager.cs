using HRMv2.Entities;
using HRMv2.Manager.Debts.PaymentPlansManager.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
using NccCore.Extension;
using NccCore.Paging;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Debts.PaymentPlansManager
{
    public class PaymentPlanManager : BaseManager
    {
        public PaymentPlanManager(IWorkScope workScope) : base(workScope)
        {
        }

        public IQueryable<PaymentPlanDto> QueryAllDebt()
        {
            return WorkScope.GetAll<DebtPaymentPlan>()
                .Select(x => new PaymentPlanDto
                {
                    Id = x.Id,
                    DebtId = x.DebtId,
                    Date = x.Date,  
                    Money = x.Money,
                    Note = x.Note,
                    PaymentType = x.PaymentType,
                    IsAllowEdit = true,
                    CreationTime = x.CreationTime,
                    CreatorUser = x.CreatorUser.FullName,
                    UpdatedTime = x.LastModificationTime,
                    UpdatedUser = x.LastModifierUser.FullName
                });
        }
        public List<PaymentPlanDto> GetAll()
        {
            return QueryAllDebt().ToList();
        }
        public PaymentPlanDto Get(long id)
        {
            return QueryAllDebt().Where(x => x.Id == id).FirstOrDefault();
        }

        public List<PaymentPlanDto> GetPaymentPlansByDebId(long debtId)
        {
            return QueryAllDebt()
                .Where(x => x.DebtId == debtId)
                .ToList();
        }

        public async Task<GridResult<PaymentPlanDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllDebt();
            return await query.GetGridResult(query, input);
        }

        public async Task<GeneratePlanDto> GeneratePlan(GeneratePlanDto input)
        {
            List<DateTime> listMonth = DateTimeUtils.GetListMonthForDebtPaymentPlan(input.StartDate, input.EndDate);
            List<DebtPaymentPlan> listToInsert = new();
            double money = input.Money / listMonth.Count;

            double roundMoney =  CommonUtil.RoundMoneyVND(money);

            var diffAmount = input.Money - (roundMoney * listMonth.Count);
            foreach (var date in listMonth.Select((value, index) => (value, index)))
            {
                var newPlan = new DebtPaymentPlan
                {
                    Date = date.value,
                    Money = roundMoney,
                    DebtId = input.DebtId,
                    PaymentType = DebtPaymentType.Salary,
                    Note = $"Trả lần {date.index + 1}"
                };
                listToInsert.Add(newPlan);
            }

            if(diffAmount != 0 && listToInsert.Count > 0) {
                var lastMoney = listToInsert[listMonth.Count - 1].Money + diffAmount;
                listToInsert[listMonth.Count - 1].Money = CommonUtil.RoundMoneyVND(lastMoney);
            }
            await WorkScope.InsertRangeAsync(listToInsert);
            return input;
        }
        //TODO: test case throw exception of create, update, delete function
        public async Task<CreatePaymentPlanDto> Create(CreatePaymentPlanDto input)
        {
            //await ValidCreate(input);
            var entity = ObjectMapper.Map<DebtPaymentPlan>(input);
            await WorkScope.InsertAsync(entity);
            return input;
        }
        public async Task<UpdatePaymentPlanDto> Update(UpdatePaymentPlanDto input)
        {
            //await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<DebtPaymentPlan>(input.Id);
            ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);
            return input;
        }
        public async Task<long> Delete(long id)
        {
            //await ValidDelete(id);
            await WorkScope.DeleteAsync<DebtPaymentPlan>(id);
            return id;
        }
    }
}
