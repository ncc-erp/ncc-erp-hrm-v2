using Abp.UI;
using Amazon.S3.Model;
using DocumentFormat.OpenXml.Wordprocessing;
using HRMv2.Entities;
using HRMv2.Manager.ChartDetails.Dto;
using HRMv2.Manager.Charts.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
<<<<<<< HEAD
=======
using NccCore.Extension;
using NccCore.Paging;
>>>>>>> origin/dev-add-chart
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.Charts
{
    public class ChartManager : BaseManager
    {
        public ChartManager(IWorkScope workScope) : base(workScope) {}

<<<<<<< HEAD
        public List<ChartDto> GetAll()
        {
            var charts = WorkScope.GetAll<Chart>().ToList();

            return ObjectMapper.Map<List<ChartDto>>(charts);
        }

        public async Task<List<ChartDto>> GetAllFilter(bool? IsActive)
        {
            var charts = WorkScope.GetAll<Chart>()
                .Where(c => IsActive == null || c.IsActive == IsActive)
                .ToList();

            return ObjectMapper.Map<List<ChartDto>>(charts);
=======
        public IQueryable<ChartDto> QueryAllChart()
        {
            var query = WorkScope.GetAll<Chart>().Select(c => new ChartDto
            {
                ChartType = c.ChartType,
                Id = c.Id,
                IsActive = c.IsActive,
                Name = c.Name,
                TimePeriodType = c.TimePeriodType
            });

            return query;
        }

        public List<ChartDto> GetAll()
        {
            var charts = QueryAllChart().ToList();

            return charts;
        }

        public async Task<GridResult<ChartDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllChart();
            var charts = await query.GetGridResult(query, input);
            return charts;
>>>>>>> origin/dev-add-chart
        }

        public async Task<ChartDto> Get(long id)
        {
            var chart = await WorkScope.GetAsync<Chart>(id);

            var chartDto = ObjectMapper.Map<ChartDto>(chart);

            return chartDto;
        }

<<<<<<< HEAD
        public async Task<ChartDto> Create(CreateChartDto createChartDto)
=======
        public async Task<Chart> Create(CreateChartDto createChartDto)
>>>>>>> origin/dev-add-chart
        {
            // validate
            var isExistedName = WorkScope.GetAll<Chart>().Any(c => c.Name == createChartDto.Name);

            if(isExistedName)
            {
                throw new UserFriendlyException($"Chart name {createChartDto.Name} is already existed");
            }

            var chart = ObjectMapper.Map<Chart>(createChartDto);

            chart.Id = await WorkScope.InsertAndGetIdAsync<Chart>(chart);
<<<<<<< HEAD

            var chartDto = ObjectMapper.Map<ChartDto>(chart);
            
            return chartDto;
        }

        public async Task<ChartDto> Update(UpdateChartDto updateChartDto)
=======
            
            return chart;
        }

        public async Task<Chart> Update(UpdateChartDto updateChartDto)
>>>>>>> origin/dev-add-chart
        {

            var chart = await WorkScope.GetAsync<Chart>(updateChartDto.Id);

            // validate
            var isExistedName = WorkScope.GetAll<Chart>().Any(c => c.Name == updateChartDto.Name && c.Id != updateChartDto.Id);

            if (isExistedName)
            {
                throw new UserFriendlyException($"Chart name {updateChartDto.Name} is already existed");
            }

            // update
<<<<<<< HEAD
            CommonUtil.MergeDataTwoEntites<UpdateChartDto, Chart>(updateChartDto, chart);

            await WorkScope.UpdateAsync(chart);

            var chartDto = ObjectMapper.Map<ChartDto>(chart);

            return chartDto;
=======
            ObjectMapper.Map<UpdateChartDto, Chart>(updateChartDto, chart);

            await WorkScope.UpdateAsync(chart);

            return chart;
>>>>>>> origin/dev-add-chart
        }

        public async Task<ChartDto> Active(long id)
        {
            var chart = await WorkScope.GetAsync<Chart>(id);

            chart.IsActive = true;

            await WorkScope.UpdateAsync<Chart>(chart);

            var chartDto = ObjectMapper.Map<ChartDto>(chart);

            return chartDto;
        }

        public async Task<ChartDto> DeActive(long id)
        {
            var chart = await WorkScope.GetAsync<Chart>(id);

            chart.IsActive = false;

            await WorkScope.UpdateAsync<Chart>(chart);

            var chartDto = ObjectMapper.Map<ChartDto>(chart);

            return chartDto;
        } 

        public async Task<long> Delete(long id)
        {
            await WorkScope.DeleteAsync<Chart>(id);

            return id;
        }
    }
}
