using Abp.UI;
using Amazon.S3.Model;
using DocumentFormat.OpenXml.Wordprocessing;
using HRMv2.Entities;
using HRMv2.Manager.ChartDetails.Dto;
using HRMv2.Manager.Charts.Dto;
using HRMv2.NccCore;
using HRMv2.Utils;
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
        }

        public async Task<ChartDto> Get(long id)
        {
            var chart = await WorkScope.GetAsync<Chart>(id);

            var chartDto = ObjectMapper.Map<ChartDto>(chart);

            return chartDto;
        }

        public async Task<ChartDto> Create(CreateChartDto createChartDto)
        {
            // validate
            var isExistedName = WorkScope.GetAll<Chart>().Any(c => c.Name == createChartDto.Name);

            if(isExistedName)
            {
                throw new UserFriendlyException($"Chart name {createChartDto.Name} is already existed");
            }

            var chart = ObjectMapper.Map<Chart>(createChartDto);

            chart.Id = await WorkScope.InsertAndGetIdAsync<Chart>(chart);

            var chartDto = ObjectMapper.Map<ChartDto>(chart);
            
            return chartDto;
        }

        public async Task<ChartDto> Update(UpdateChartDto updateChartDto)
        {

            var chart = await WorkScope.GetAsync<Chart>(updateChartDto.Id);

            // validate
            var isExistedName = WorkScope.GetAll<Chart>().Any(c => c.Name == updateChartDto.Name && c.Id != updateChartDto.Id);

            if (isExistedName)
            {
                throw new UserFriendlyException($"Chart name {updateChartDto.Name} is already existed");
            }

            // update
            CommonUtil.MergeDataTwoEntites<UpdateChartDto, Chart>(updateChartDto, chart);

            await WorkScope.UpdateAsync(chart);

            var chartDto = ObjectMapper.Map<ChartDto>(chart);

            return chartDto;
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
