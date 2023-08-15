using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.PunishmentFunds.Dto;
using HRMv2.NccCore;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.PunishmentFunds
{
    public class PunishmentFundManager : BaseManager
    {
        public PunishmentFundManager(IWorkScope workScope) : base(workScope)
        {
        }

        public IQueryable<GetAllPunishmentFundsDto> QueryAllPunishmentFund()
        {
            return WorkScope.GetAll<PunishmentFund>()
                .Select(x => new GetAllPunishmentFundsDto
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    Date = x.Date,
                    Note = x.Note,
                    CreationTime = x.CreationTime,
                    CreationUser = x.CreatorUser.FullName,
                    LastModificationTime = x.LastModificationTime,
                    LastModifierUser = x.LastModifierUser != null ? x.LastModifierUser.FullName : "",
                });
        }

        public async Task<GridResult<GetAllPunishmentFundsDto>> GetAllPaging(InputToGetAllPagingDto input)
        {
            var query = QueryAllPunishmentFund();
            if (input.FilterByComparision != null)
            {
                double amount = Double.Parse(input.FilterByComparision.Value);
                switch (input.FilterByComparision.OperatorComparison)
                {
                    case Comparision.Equal:
                        {
                            query = query.Where(x => x.Amount == amount);
                            break;
                        };
                    case Comparision.GreaterThan:
                        {
                            query = query.Where(x => x.Amount > amount);
                            break;
                        }
                    case Comparision.LessThan:
                        {
                            query = query.Where(x => x.Amount < amount);
                            break;
                        }
                }
            }
            query = query.OrderByDescending(x => x.Date);
            return await query.GetGridResult(query, input.GridParam);
        }


        public async Task<AddEditPunishmentFundDto> Add(AddEditPunishmentFundDto input)
        {
            var entity = ObjectMapper.Map<PunishmentFund>(input);
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);
            return input;

        }

        public async Task<AddEditPunishmentFundDto> Update(AddEditPunishmentFundDto input)
        {
            var entity = await WorkScope.GetAsync<PunishmentFund>(input.Id);
            if (entity == default)
            {
                throw new UserFriendlyException($"Can not found punishment fund with Id = {input.Id}");
            }
            ObjectMapper.Map(input, entity);
            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public async Task<long> Delete(long Id)
        {
            var entity = await WorkScope.GetAsync<PunishmentFund>(Id);
            if (entity == default)
            {
                throw new UserFriendlyException($"Can not found punishment fund with Id = {Id}");
            }
            await WorkScope.DeleteAsync<PunishmentFund>(Id);
            return Id;
        }

        public async Task<AddEditPunishmentFundDto> Disburse(AddEditPunishmentFundDto input)
        {
            var entity = ObjectMapper.Map<PunishmentFund>(input);
            entity.Amount = -input.Amount;
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);
            return input;
        }

        public double GetFundCurrentBalance()
        {
            return WorkScope.GetAll<PunishmentFund>()
                 .Sum(x => x.Amount);
        }

    }
}
