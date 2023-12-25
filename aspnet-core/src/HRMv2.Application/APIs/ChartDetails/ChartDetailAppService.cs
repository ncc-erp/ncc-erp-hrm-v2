using HRMv2.Manager.ChartDetails.Dto;
using HRMv2.Manager.ChartDetails;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NccCore.Paging;
using HRMv2.Entities;

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
        [HttpPost]
        public async Task<GridResult<ChartDetailDto>> GetAllPaging(GridParam input)
        {
            var chartDetails = await _chartDetailManager.GetAllPaging(input);

            return chartDetails;
        }

        [HttpGet]
        public async Task<ChartDetailDto> Get(long id)
        {
            var chartDetail = await _chartDetailManager.Get(id);

            return chartDetail;
        }

        [HttpPost]
        public async Task<ChartDetail> Create(CreateChartDetailDto createChartDetailDto)

        {
            var chartDetail = await _chartDetailManager.Create(createChartDetailDto);

            return chartDetail;
        }

        [HttpPut]
        public async Task<ChartDetail> Update(UpdateChartDetailDto updateChartDetailDto)

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
