using Abp.Authorization;
using Abp.Dependency;
using HRMv2.Configuration;
using HRMv2.Manager.Categories.Teams;
using HRMv2.Manager.Debts;
using HRMv2.Manager.Debts.Dto;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Home;
using HRMv2.Manager.PunishmentFunds;
using HRMv2.Manager.PunishmentFunds.Dto;
using HRMv2.NCC;
using HRMv2.WebServices.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NccCore.Paging;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.APIs.Publics
{
   public class PublicAppService : HRMv2AppServiceBase
   {
      private readonly TeamManager _teamManager;
      private readonly EmployeeManager _employeeManager;
      private readonly PunishmentFundManager _punishmentFundsManager;
      protected IHttpContextAccessor _httpContextAccessor { get; set; }
      private readonly HomePageManager _homePageManager;
      private readonly DebtManager _debtManager;


      public PublicAppService(EmployeeManager employeeManager, IConfiguration configuration,
          PunishmentFundManager punishmentFundsManager,
          HomePageManager homePageManager,
          TeamManager teamManager,
          DebtManager debtManager)
      {
         _employeeManager = employeeManager;
         _punishmentFundsManager = punishmentFundsManager;
         _homePageManager = homePageManager;
         _httpContextAccessor = IocManager.Instance.Resolve<IHttpContextAccessor>();
         _debtManager = debtManager;
         _teamManager = teamManager;

      }

      [HttpGet]
      public List<EmployeeHasBirthdayInMonthDto> GetEmployeesBirthdayInMonth(int month)
      {
         return _employeeManager.GetEmployeesBirthdayInMonth(month);
      }

      [HttpGet]
      public List<EmployeeHasBirthdayInMonthDto> GetEmployeesByBirthday(int month, int day)
      {
         return _employeeManager.GetEmployeesByBirthday(month, day);
      }

      [HttpGet]
      public GetEmployeeStatisticDto GetEmployeeStatistic(InputExportEmployeeStatisticDto input)
      {
         return _employeeManager.GetEmployeeStatistic(input);
      }

      [HttpGet]
      public GetResultConnectDto CheckConnect()
      {
         //var secretCode = SettingManager.GetSettingValue(AppSettingNames.SecurityCode);
         var secretCode = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .Build()
             .GetValue<string>($"App:SecurityCode");
         var header = _httpContextAccessor.HttpContext.Request.Headers;

         var securityCodeHeader = header["X-Secret-Key"];
         if (string.IsNullOrEmpty(securityCodeHeader))
         {
            securityCodeHeader = header["securityCode"];
         }
         var result = new GetResultConnectDto();
         if (secretCode != securityCodeHeader)
         {
            result.IsConnected = false;
            result.Message = $"SecretCode does not match: " + securityCodeHeader + " != ***" + secretCode.Substring(secretCode.Length - 3);
            return result;
         }
         result.IsConnected = true;
         result.Message = "Connected";
         return result;
      }
      [HttpGet]
      [NccAuthentication]
      public Task<GetPhoneNumber> GetEmployeePhone(string email)
      {
         return _employeeManager.GetEmployeePhone(email);
      }
      [HttpGet]
      [NccAuthentication]
      public double GetFundCurrentBalance()
      {
         return _punishmentFundsManager.GetFundCurrentBalance();
      }

      [HttpPost]
      [NccAuthentication]
      public async Task<GridResult<GetAllPunishmentFundsDto>> GetFunAmountHistories(InputToGetAllPagingDto input)
      {
         return await _punishmentFundsManager.GetAllPaging(input);
      }


      [AbpAuthorize]
      public object CheckHomepage()
      {
         var input = new GetEmployeeToAddDto
         {
            AddedEmployeeIds = new List<long>(),
            BranchIds = new List<long>(),
            IsAndCondition = false,
            StatusIds = new List<Constants.Enum.HRMEnum.EmployeeStatus> { Constants.Enum.HRMEnum.EmployeeStatus.Working },
            TeamIds = new List<long>(),
            Usertypes = new List<Constants.Enum.HRMEnum.UserType>(),
            GridParam = new GridParam
            {
               MaxResultCount = 1000,
               SkipCount = 0
            }
         };
         var employees = _employeeManager.GetEmployeeExcept(input);

         var emails = employees.Result.Items.Select(s => s.Email).ToList();

         var endDate = DateTimeUtils.GetNow();
         var startDate = endDate.AddMonths(-1);

         var homepage = _homePageManager.GetLastEmployeeWorkingHistories(startDate, endDate);

         var hpEmails = homepage.Where(s => s.LastStatus == Constants.Enum.HRMEnum.EmployeeStatus.Working).Select(s => s.Email).ToList();


         return new
         {
            HomePageOnlyList = hpEmails.Except(emails),
            EmployeePageOnlyList = emails.Except(hpEmails),
         };
      }

      [NccAuthentication]
      public List<GetAllEmployeeDto> GetAllEmployee()
      {
         var employees = _employeeManager.GetAllEmployee();
         return employees;
      }

      [NccAuthentication]
      public Task<GetEmployeeByEmailDto> GetEmployeeByEmail(string email)
      {
         var employees = _employeeManager.GetEmployeeByEmail(email);
         return employees;
      }

      [NccAuthentication]
      public GetAllDebtEmployeeDto GetAllDebtEmployee()
      {
         return _debtManager.GetAllDebtEmployee();
      }

      [NccAuthentication]
      public List<TeamDto> GetAllTeam()
      {
         return _teamManager.GetAll();
      }
   }
}
