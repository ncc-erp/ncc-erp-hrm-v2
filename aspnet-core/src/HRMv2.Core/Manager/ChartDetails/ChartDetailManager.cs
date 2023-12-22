using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.ChartDetails.Dto;
using HRMv2.Manager.Charts;
using HRMv2.NccCore;
using HRMv2.Utils;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.ChartDetails
{
    public class ChartDetailManager : BaseManager
    {
        protected readonly ChartManager _chartManager;

        public ChartDetailManager(IWorkScope workScope,
            ChartManager chartManager) : base(workScope)
        {
            _chartManager = chartManager;
        }

        public IQueryable<ChartDetailDto> QueryAll ()
        {
            var query = WorkScope.GetAll<ChartDetail>()
                .Select(x => new ChartDetailDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ChartId = x.ChartId,
                    Color = x.Color,
                    IsActive = x.IsActive,
                    BranchIds = x.BranchIds,
                    JobPositionIds = x.JobPositionIds,
                    LevelIds = x.LevelIds,
                    TeamIds = x.TeamIds,
                    UserTypes = x.UserTypes,
                    PayslipDetailTypes = x.PayslipDetailTypes,
                    WorkingStatuses = x.WorkingStatuses
                });

            return query;
        }

        public List<ChartDetailDto> GetAll()
        {
            var chartDetails = QueryAll().ToList();

            return ObjectMapper.Map<List<ChartDetailDto>>(chartDetails);
        }

        public async Task<GridResult<ChartDetailDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAll();
            var result = await query.GetGridResult(query, input);

            return result;
        }

        public async Task<ChartDetailDto> Get(long id)
        {
            var chartDetail = await WorkScope.GetAsync<ChartDetail>(id);

            var chartDetailDto = ObjectMapper.Map<ChartDetailDto>(chartDetail);

            return chartDetailDto;
        }

        public async Task<ChartDetail> Create(CreateChartDetailDto createChartDetailDto)
        {
            //var chart = await _chartManager.Get(createChartDetailDto.ChartId);

            // validate
            var isExistedNameInChart = WorkScope.GetAll<ChartDetail>().Where(cd => cd.ChartId == createChartDetailDto.ChartId).Any(cd => cd.Name == createChartDetailDto.Name);

            if (isExistedNameInChart)
            {
                throw new UserFriendlyException($"ChartDetail name {createChartDetailDto.Name} is already existed in ChartID: {createChartDetailDto.ChartId}");
            }

            var chartDetail = ObjectMapper.Map<ChartDetail>(createChartDetailDto);

            chartDetail.Id = await WorkScope.InsertAndGetIdAsync<ChartDetail>(chartDetail);

            return chartDetail;
        }

        public async Task<ChartDetail> Update(UpdateChartDetailDto updateChartDetailDto)
        {

            var chartDetail = await WorkScope.GetAsync<ChartDetail>(updateChartDetailDto.Id);

            // validate
            var isExistedName = WorkScope.GetAll<ChartDetail>().Any(c => c.Name == updateChartDetailDto.Name && c.Id != updateChartDetailDto.Id);

            if (isExistedName)
            {
                throw new UserFriendlyException($"ChartDetail name {updateChartDetailDto.Name} is already existed");
            }

            // update
            await WorkScope.UpdateAsync(chartDetail);

            return chartDetail;
        }


        public async Task<ChartDetailDto> Active(long id)
        {
            var chartDetail = await WorkScope.GetAsync<ChartDetail>(id);

            chartDetail.IsActive = true;

            await WorkScope.UpdateAsync<ChartDetail>(chartDetail);

            var chartDetailDto = ObjectMapper.Map<ChartDetailDto>(chartDetail);

            return chartDetailDto;
        }

        public async Task<ChartDetailDto> DeActive(long id)
        {
            var chartDetail = await WorkScope.GetAsync<ChartDetail>(id);

            chartDetail.IsActive = false;

            await WorkScope.UpdateAsync<ChartDetail>(chartDetail);

            var chartDetailDto = ObjectMapper.Map<ChartDetailDto>(chartDetail);

            return chartDetailDto;
        }

        public async Task<long> Delete(long id)
        {
            await WorkScope.DeleteAsync<ChartDetail>(id);

            return id;
        }
    }
}
