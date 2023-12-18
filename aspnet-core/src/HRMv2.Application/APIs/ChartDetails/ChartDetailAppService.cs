using HRMv2.Manager.ChartDetails.Dto;
using HRMv2.Manager.ChartDetails;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.ChartDetails
{
    public class ChartDetailAppService : HRMv2AppServiceBase
    {
        private readonly ChartDetailManager _chartDetailManager;

        public ChartDetailAppService(ChartDetailManager chartDetailManager)
        {
            _chartDetailManager = chartDetailManager;
        }

        [HttpGet]
        public async Task<List<ChartDetailDto>> GetAll()
        {
            var chartDetails = _chartDetailManager.GetAll();

            return chartDetails;
        }

        [HttpGet]
        public async Task<List<ChartDetailDto>> GetAllFilter(bool? isActive)
        {
            var chartDetails = await _chartDetailManager.GetAllFilter(isActive);

            return chartDetails;
        }

        [HttpGet]
        public async Task<ChartDetailDto> Get(long id)
        {
            var chartDetail = await _chartDetailManager.Get(id);

            return chartDetail;
        }

        [HttpPost]
        public async Task<ChartDetailDto> Create(CreateChartDetailDto createChartDetailDto)
        {
            var chartDetail = await _chartDetailManager.Create(createChartDetailDto);

            return chartDetail;
        }

        [HttpPut]
        public async Task<ChartDetailDto> Update(UpdateChartDetailDto updateChartDetailDto)
        {
            var chartDetail = await _chartDetailManager.Update(updateChartDetailDto);

            return chartDetail;
        }

        [HttpPut]
        public async Task<ChartDetailDto> Active(long id)
        {
            var chartDetail = await _chartDetailManager.Active(id);

            return chartDetail;
        }

        [HttpPut]
        public async Task<ChartDetailDto> DeActive(long id)
        {
            var chartDetail = await _chartDetailManager.DeActive(id);

            return chartDetail;
        }

        [HttpDelete]
        public async Task<long> Delete(long id)
        {
            var chartDetailId = await _chartDetailManager.Delete(id);

            return chartDetailId;
        }
    }
}
