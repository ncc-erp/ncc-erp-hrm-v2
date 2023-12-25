﻿using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.ChartDetails.Dto;
using HRMv2.Manager.Charts;
<<<<<<< HEAD
using HRMv2.NccCore;
using HRMv2.Utils;
=======
using HRMv2.Manager.Charts.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
using NccCore.Extension;
using NccCore.Paging;
>>>>>>> origin/dev-add-chart
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

<<<<<<< HEAD
        public List<ChartDetailDto> GetAll()
        {
            var chartDetails = WorkScope.GetAll<ChartDetail>().ToList();

            return ObjectMapper.Map<List<ChartDetailDto>>(chartDetails);
        }

        public async Task<List<ChartDetailDto>> GetAllFilter(bool? IsActive)
        {
            var chartDetails = WorkScope.GetAll<ChartDetail>()
                .Where(c => IsActive == null || c.IsActive == IsActive)
                .ToList();

            return ObjectMapper.Map<List<ChartDetailDto>>(chartDetails);
=======
        public IQueryable<ChartDetailDto> QueryAllChartDetail()
        {
            var query = WorkScope.GetAll<ChartDetail>().Select(c => new ChartDetailDto
            {
                Id = c.Id,
                IsActive = c.IsActive,
                Name = c.Name,
                BranchIds = c.BranchIds,
                ChartId = c.ChartId,
                Color = c.Color,
                JobPositionIds = c.JobPositionIds,
                LevelIds = c.LevelIds,
                PayslipDetailTypes = c.PayslipDetailTypes,
                TeamIds = c.TeamIds,
                UserTypes = c.UserTypes,
                WorkingStatuses = c.WorkingStatuses
            });

            return query;
        }

        public List<ChartDetailDto> GetAll()
        {
            var chartDetails = QueryAllChartDetail().ToList();

            return chartDetails;
        }

        public async Task<GridResult<ChartDetailDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllChartDetail();
            var chartDetails = await query.GetGridResult(query, input);
            return chartDetails;
>>>>>>> origin/dev-add-chart
        }

        public async Task<ChartDetailDto> Get(long id)
        {
            var chartDetail = await WorkScope.GetAsync<ChartDetail>(id);

            var chartDetailDto = ObjectMapper.Map<ChartDetailDto>(chartDetail);

            return chartDetailDto;
        }

<<<<<<< HEAD
        public async Task<ChartDetailDto> Create(CreateChartDetailDto createChartDetailDto)
=======
        public async Task<ChartDetail> Create(CreateChartDetailDto createChartDetailDto)
>>>>>>> origin/dev-add-chart
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

<<<<<<< HEAD
            var chartDetailDto = ObjectMapper.Map<ChartDetailDto>(chartDetail);

            return chartDetailDto;
        }

        public async Task<ChartDetailDto> Update(UpdateChartDetailDto updateChartDetailDto)
=======
            return chartDetail;
        }

        public async Task<ChartDetail> Update(UpdateChartDetailDto updateChartDetailDto)
>>>>>>> origin/dev-add-chart
        {

            var chartDetail = await WorkScope.GetAsync<ChartDetail>(updateChartDetailDto.Id);

            // validate
            var isExistedName = WorkScope.GetAll<ChartDetail>().Any(c => c.Name == updateChartDetailDto.Name && c.Id != updateChartDetailDto.Id);

            if (isExistedName)
            {
                throw new UserFriendlyException($"ChartDetail name {updateChartDetailDto.Name} is already existed");
            }

            // update
<<<<<<< HEAD
            CommonUtil.MergeDataTwoEntites<UpdateChartDetailDto, ChartDetail>(updateChartDetailDto, chartDetail);

            await WorkScope.UpdateAsync(chartDetail);

            var chartDetailDto = ObjectMapper.Map<ChartDetailDto>(chartDetail);

            return chartDetailDto;
=======
            ObjectMapper.Map<UpdateChartDetailDto, ChartDetail>(updateChartDetailDto, chartDetail);

            await WorkScope.UpdateAsync(chartDetail);

            return chartDetail;
>>>>>>> origin/dev-add-chart
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
