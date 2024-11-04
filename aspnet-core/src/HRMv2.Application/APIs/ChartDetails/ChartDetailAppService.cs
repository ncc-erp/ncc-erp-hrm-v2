﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NccCore.Paging;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Charts.ChartDetails;
using HRMv2.Manager.Categories.Charts.Dto;
using HRMv2.Manager.Categories.Charts.ChartDetails.Dto;
using Abp.Authorization;
using HRMv2.Authorization;

namespace HRMv2.APIs.ChartDetails
{
    [AbpAuthorize]
    public class ChartDetailAppService : HRMv2AppServiceBase
    {
        private readonly ChartDetailManager _chartDetailManager;

        public ChartDetailAppService(ChartDetailManager chartDetailManager)
        {
            _chartDetailManager = chartDetailManager;
        }

        #region Get
        [HttpGet]
        [AbpAuthorize(PermissionNames.Category_Chart_ChartDetail_View)]
        public async Task<ChartDetailFullDto> Get(long id)
        {
            var chartDetail = await _chartDetailManager.Get(id);

            return chartDetail;
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Category_Chart_ChartDetail_View)]
        public async Task<ChartFullInfoDto> GetAllDetailsByChartId(long id)
        {
            var chartFullDetail = await _chartDetailManager.GetAllDetailsByChartId(id);

            return chartFullDetail;
        }

        [HttpGet]
        [AbpAuthorize(PermissionNames.Category_Chart_ChartDetail_View)]
        public ChartDetailSelectionDto GetChartDetailSelectionData()
        {
            var selectionData = _chartDetailManager.GetChartDetailSelectionData();

            return selectionData;
        } 
        #endregion

        [HttpPost]
        [AbpAuthorize(PermissionNames.Category_Chart_ChartDetail_Create)]
        public async Task<ChartDetail> Create(CreateChartDetailDto createChartDetailDto)
        {
            var chartDetail = await _chartDetailManager.Create(createChartDetailDto);

            return chartDetail;
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Category_Chart_ChartDetail_Edit)]
        public async Task<ChartDetail> Update(UpdateChartDetailDto updateChartDetailDto)
        {
            var chartDetail = await _chartDetailManager.Update(updateChartDetailDto);

            return chartDetail;
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Category_Chart_ChartDetail_ActiveDeactive)]
        public async Task<ChartDetailDto> Active([FromBody] long id)
        {
            var chartDetail = await _chartDetailManager.Active(id);

            return chartDetail;
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.Category_Chart_ChartDetail_ActiveDeactive)]
        public async Task<ChartDetailDto> DeActive([FromBody] long id)
        {
            var chartDetail = await _chartDetailManager.DeActive(id);

            return chartDetail;
        }

        [HttpDelete]
        [AbpAuthorize(PermissionNames.Category_Chart_ChartDetail_Delete)]
        public async Task<long> Delete(long id)
        {
            var chartDetailId = await _chartDetailManager.Delete(id);

            return chartDetailId;
        }

        
    }
}