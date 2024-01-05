
using HRMv2.Entities;
using HRMv2.Manager.Categories.Charts;
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
    public class ChartAppService : HRMv2AppServiceBase
    {
        private readonly ChartManager _chartManager;

        public ChartAppService(ChartManager chartManager)
        {
            _chartManager = chartManager;
        }

        [HttpGet]
        public async Task<List<ChartDto>> GetAll()
        {
            var charts = _chartManager.GetAll();

            return charts;
        }


        [HttpPost]
        public async Task<GridResult<ChartDto>> GetAllPaging(GridParam input)
        {
            var charts = await _chartManager.GetAllPaging(input);

            return charts;
        }

        [HttpGet]
        public async Task<ChartDto> Get(long id)
        {
            var chart = await _chartManager.Get(id);

            return chart;
        }

        [HttpPost]
        public async Task<Chart> Create(CreateChartDto createChartDto)
        {
            var chart = await _chartManager.Create(createChartDto);

            return chart;
        }

        [HttpPut]

        public async Task<Chart> Update(UpdateChartDto updateChartDto)
        {
            var chart = await _chartManager.Update(updateChartDto);

            return chart;
        }

        [HttpPut]
        public async Task<ChartDto> Active([FromBody] long id)
        {
            var chart = await _chartManager.Active(id);
            
            return chart;
        }

        [HttpPut]
        public async Task<ChartDto> DeActive([FromBody] long id)
        {
            var chart = await _chartManager.DeActive(id);

            return chart;
        }

        [HttpDelete]
        public async Task<long> Delete(long id)
        {
            var chartId = await _chartManager.Delete(id);

            return chartId;
        }
    }
}
