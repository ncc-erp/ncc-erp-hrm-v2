using Abp.BackgroundJobs;
using Abp.UI;
using HRMv2.BackgroundJob.SendDirectMessage;
using HRMv2.BackgroundJob.SentToken;
using HRMv2.Constants;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.MezonTokens.Dto;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.SendMezonDM;
using HRMv2.Manager.Report.Dto;
using HRMv2.Manager.Salaries.Dto;
using HRMv2.Manager.Salaries.Payrolls.Dto;
using HRMv2.Manager.Salaries.Payslips.Dto;
using HRMv2.MMN;
using HRMv2.NccCore;
using HRMv2.Net.MimeTypes;
using HRMv2.WebServices.Mezon;
using HRMv2.WebServices.Mezon.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using MmnDotNetSdk.Utils;
using NccCore.Extension;
using NccCore.Paging;
using NccCore.Uitls;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.MezonTokens
{
    public class MezonTokenManager : BaseManager
    {
        private readonly MezonWebService _mezonWebService;
        private readonly BackgroundJobManager _backgroundJobManager;
        private readonly IConfiguration _configuration;
        private readonly SendMezonDMService _sendDMService;
        private readonly EmailManager _emailManager;
        private readonly MmnService _mmnService;
        private static readonly Queue<InputSendMezonToken> _tokenQueue = new();

        public MezonTokenManager(IWorkScope workScope, MezonWebService mezonWebService,
            BackgroundJobManager backgroundJobManager, IConfiguration configuration
            , SendMezonDMService sendDMService,
            EmailManager emailManager,
            MmnService mmnService) : base(workScope)
        {
            _mezonWebService = mezonWebService;
            _backgroundJobManager = backgroundJobManager;
            _configuration = configuration;
            _sendDMService = sendDMService;
            _emailManager = emailManager;
            _mmnService = mmnService;
        }
        public async Task<ResultMezonToken> GetAllPaging(InputMezonToken input)
        {

            var query = GetAllMezonToken();

            if (input.PayrollIds != null && input.PayrollIds.Any())
            {
                query = query.Where(x => (x.PayrollId == null && input.PayrollIds.Contains(-1)) ||
                 (x.PayrollId != null && input.PayrollIds.Contains(x.PayrollId.Value)));
            }



            var queryFilter = query.ApplySearchAndFilter(input.GridParam);
            var totalToken = queryFilter.Sum(x => x.Amount);


            var totalCount = queryFilter.Count();

            var pagedResult = await queryFilter.TakePage(input.GridParam).ToListAsync();
            return new ResultMezonToken
            {
                Result = new GridResult<MezonTokenDto>(pagedResult, totalCount),
                TotalAmout = totalToken,
            };
        }

        public IQueryable<MezonTokenDto> GetAllMezonToken()
        {
            return WorkScope.GetAll<MezonToken>()
                  .Include(x => x.Employee)
                  .OrderByDescending(x => x.CreationTime)
                  .Select(x => new MezonTokenDto()
                  {
                      Id = x.Id,
                      Note = x.Note,
                      Amount = x.Amount,
                      SentToEmployeeAt = x.SentToEmployeeAt,
                      StatusToken = x.Status,
                      FullName = x.Employee.FullName,
                      Email = x.Employee.Email,
                      Avatar = x.Employee.Avatar,
                      Sex = x.Employee.Sex,
                      EmployeeId = x.EmployeeId,
                      BranchInfo = new BadgeInfoDto
                      {
                          Name = x.Employee.Branch.Name,
                          Color = x.Employee.Branch.Color
                      },
                      LevelInfo = new BadgeInfoDto
                      {
                          Name = x.Employee.Level.Name,
                          Color = x.Employee.Level.Color
                      },
                      JobPositionInfo = new BadgeInfoDto
                      {
                          Name = x.Employee.JobPosition.Name,
                          Color = x.Employee.JobPosition.Color
                      },
                      ReferenceId = x.ReferenceId,
                      PayrollId = x.PayrollId,
                      PayrollApplyMonth = x.Payroll != null ? x.Payroll.ApplyMonth : null
                  });
        }

        public async Task<MezonTokenDto> Create(MezonTokenDto input)
        {
            var entity = ObjectMapper.Map<MezonToken>(input);
            entity.Status = StatusSendToken.Pending;
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);
            return input;
        }

        public async Task DeleteMezonTokenById(long mezonTokenId)
        {
            var mezonToken = WorkScope.GetAll<MezonToken>().FirstOrDefault(x => x.Id == mezonTokenId);
            mezonToken.IsDeleted = true;
            await WorkScope.UpdateAsync(mezonToken);

        }

        public async Task DeleteAllPending()
        {
            var mezonTokens = WorkScope.GetAll<MezonToken>()
                .Where(x => x.Status == StatusSendToken.Pending).ToList();
            mezonTokens.ForEach(x => x.IsDeleted = true);
            await WorkScope.UpdateRangeAsync(mezonTokens);
        }

        public async Task<MezonTokenDto> EditMezonToken(MezonTokenDto input)
        {
            var entity = await WorkScope.GetAsync<MezonToken>(input.Id);
            entity.Amount = input.Amount;
            entity.EmployeeId = input.EmployeeId;
            entity.Note = input.Note;
            entity.PayrollId = input.PayrollId;
            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public List<ExportMezonTokenDto> GetFilterMezonToken(InputMezonToken input)
        {
            var mezonTokens = GetAllPaging(input).Result;
            return mezonTokens.Result.Items
                .Select(x => new ExportMezonTokenDto
                {
                    EmailAddress = x.Email,
                    Amount = x.Amount,
                    Note = x.Note,
                    Status = Enum.GetName(typeof(StatusSendToken), x.StatusToken),
                    SentAt = x.SentToEmployeeAt,
                })
                .ToList();
        }
        public async Task<FileBase64Dto> ExportMezonToken(InputMezonToken input)
        {
            var templateFilePath = Path.Combine(HRMv2Consts.templateFolder, "Export-MezonToken.xlsx");

            if (templateFilePath == default)
            {
                throw new UserFriendlyException("Can't find template Export-MezonToken.xlsx");
            }

            input.GridParam.MaxResultCount = int.MaxValue;
            input.GridParam.SkipCount = 0;
            var mezonTokens = GetFilterMezonToken(input);

            using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {
                using (var template = new ExcelPackage(memoryStream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    FillDataToExport(template, mezonTokens);

                    string fileBase64 = Convert.ToBase64String(template.GetAsByteArray());

                    return new FileBase64Dto
                    {
                        FileName = "MezonToken",
                        FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                        Base64 = fileBase64
                    };
                }
            }
        }

        private void FillDataToExport(ExcelPackage package, List<ExportMezonTokenDto> data)
        {
            var onboardMezonTokenSheet = package.Workbook.Worksheets[0];
            var onboardRowIndex = 2;

            foreach (var mezonToken in data)
            {
                onboardMezonTokenSheet.Cells[onboardRowIndex, 1].Value = onboardRowIndex - 1;
                onboardMezonTokenSheet.Cells[onboardRowIndex, 2].Value = mezonToken.EmailAddress;
                onboardMezonTokenSheet.Cells[onboardRowIndex, 3].Value = mezonToken.Amount;
                onboardMezonTokenSheet.Cells[onboardRowIndex, 4].Value = mezonToken.Note;
                onboardMezonTokenSheet.Cells[onboardRowIndex, 5].Value = mezonToken.Status;
                onboardMezonTokenSheet.Cells[onboardRowIndex, 6].Value = mezonToken.SentAt;

                onboardRowIndex++;
            }

        }


        private void SendNotiDM(long mezonTokenId)
        {
            var sendContent = _emailManager.GetDMMezonContentById(NotifyTemplateEnum.MezonDMSendToken, mezonTokenId);
            _sendDMService.SendDMToUser(sendContent);
        }

        public async Task<AuthResponse> SendToken(InputSendMezonToken input)
        {


            var mezonTokenInfo = await WorkScope.GetAll<MezonToken>()
            .Select(s => new
            {
                MezonToken = s,
                s.Employee.Email,
                s.Employee.UserMezonId
            })
            .FirstOrDefaultAsync(x => x.MezonToken.Id == input.MezonTokenId);

            if (mezonTokenInfo == null)
            {
                throw new UserFriendlyException("Mezon Token doesn't exist");
            }


            var userName = mezonTokenInfo.Email.Split("@")[0];

            var mmnTransferTokenDto = new MmnTransferTokenDto
            {
                sender_id = MezonTokenConstant.BotId,
                amount = mezonTokenInfo.MezonToken.Amount,
                receiver_id = mezonTokenInfo.UserMezonId,
                note = mezonTokenInfo.MezonToken.Note,
            };

            if (string.IsNullOrEmpty(mmnTransferTokenDto.JwtTokenBot))
            {
                var tokenResponse = await _mezonWebService.GetAuthDataMezon();
                mmnTransferTokenDto.JwtTokenBot = tokenResponse.token;
            }

            var response = await _mmnService.TransferToken(mmnTransferTokenDto);

            if (response.Ok)
            {
                mezonTokenInfo.MezonToken.SentToEmployeeAt = DateTimeUtils.GetNow();
                mezonTokenInfo.MezonToken.Status = StatusSendToken.SentToEmployee;
                mezonTokenInfo.MezonToken.Note = $"{mezonTokenInfo.MezonToken.Note}\n MmnTxn: {response.TxHash}";
                await WorkScope.UpdateAsync(mezonTokenInfo.MezonToken);

                SendNotiDM(input.MezonTokenId);
                return new AuthResponse
                {
                    code = 0,
                    message = $"Sent Token to {userName} successfully!"
                };
            }

            return new AuthResponse
            {
                code = 1,
                message = $"Failed to send Token to {userName} error: {response.Error}"
            };
        }

        public async Task<string> SendTokenToAllPending()
        {

            var authData = await _mezonWebService.GetAuthDataMezon();
            var input = WorkScope.GetAll<MezonToken>()
                .Where(x => x.Status == StatusSendToken.Pending)
                .Select(x => new InputSendMezonToken
                {
                    MezonTokenId = x.Id,
                    JwtTokenBot = authData.token
                })
                .ToList();

            var delaySendToken = 0;

            foreach (var item in input)
            {
                _backgroundJobManager.Enqueue<SendMezonTokenBackgroundJob, InputSendMezonToken>(item, BackgroundJobPriority.High, TimeSpan.FromSeconds(delaySendToken));
                delaySendToken += 1;
            }


            return $"Started sending token to {input.Count} users.";
        }
    }
}
