
using Abp.Authorization;
using HRMv2.Authorization;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Charts;
using HRMv2.Manager.Categories.Charts.ChartDetails.Dto;
using HRMv2.Manager.Categories.Charts.ChartDetails;
using HRMv2.Manager.Categories.Charts.Dto;
using Microsoft.AspNetCore.Mvc;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Charts
{
    [AbpAuthorize]
    public class ChartAppService : HRMv2AppServiceBase
    {
        private readonly ChartManager _chartManager;

        public ChartAppService(ChartManager chartManager)
        {
            _chartManager = chartManager;
        }

        [HttpGet]
        public ChartSelectionDto GetChartSelectionData()
        {
            var selectionData = _chartManager.GetChartSelectionData();

            return selectionData;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_Chart_View)]
        public async Task<GridResult<ChartDto>> GetAllPaging(GridParam input)
        {
            var charts = await _chartManager.GetAllPaging(input);

            return charts;
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Category_Chart_View)]
        public async Task<ChartDto> Get(long id)
        {
            var chart = await _chartManager.Get(id);

            return chart;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_Chart_Create)]
        public async Task<Chart> Create(CreateChartDto createChartDto)
        {
            var chart = await _chartManager.Create(createChartDto);

            return chart;
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Category_Chart_Edit)]
        public async Task<Chart> Update(UpdateChartDto updateChartDto)
        {
            var chart = await _chartManager.Update(updateChartDto);

            return chart;
        }

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_Chart_Edit)]
        public async Task<long> Clone([FromBody] long id)
        {
            return await _chartManager.Clone(id);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Category_Chart_ActiveDeactive)]
        public async Task<ChartDto> Active([FromBody] long id)
        {
            var chart = await _chartManager.Active(id);
            
            return chart;
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Category_Chart_ActiveDeactive)]
        public async Task<ChartDto> DeActive([FromBody] long id)
        {
            var chart = await _chartManager.DeActive(id);

            return chart;
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Category_Chart_Delete)]
        public async Task<long> Delete(long id)
        {
            var chartId = await _chartManager.Delete(id);

            return chartId;
        }
    }
}
