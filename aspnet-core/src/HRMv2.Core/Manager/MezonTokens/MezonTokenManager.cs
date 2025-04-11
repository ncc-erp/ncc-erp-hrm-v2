using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.MezonTokens.Dto;
using HRMv2.Manager.Salaries.Payslips.Dto;
using HRMv2.NccCore;
using HRMv2.Net.MimeTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NccCore.Extension;
using NccCore.Paging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager.MezonTokens
{
    public class MezonTokenManager : BaseManager
    {
        public MezonTokenManager(IWorkScope workScope) : base(workScope)
        {
        }
        public async Task<GridResult<MezonTokenDto>> GetAllPaging(GridParam input)
        {
            var query = GetAllMezonToken();
            return await query.GetGridResult(query, input);
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
                  });
        }

        public async Task<MezonTokenDto> Create(MezonTokenDto input)
        {
            var entity = ObjectMapper.Map<MezonToken>(input);
            entity.Status = Constants.Enum.HRMEnum.StatusSendToken.Pending;
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);
            return input;
        }

        public async Task DeleteMezonTokenById(long mezonTokenId)
        {
            var mezonToken = WorkScope.GetAll<MezonToken>().FirstOrDefault(x => x.Id == mezonTokenId);
            mezonToken.IsDeleted = true;
            WorkScope.UpdateAsync(mezonToken);
            CurrentUnitOfWork.SaveChanges();
        }

        public async Task DeleteAllPending()
        {
            var mezonTokens = WorkScope.GetAll<MezonToken>()
                .Where(x => x.Status == Constants.Enum.HRMEnum.StatusSendToken.Pending).ToList();
            mezonTokens.ForEach(x => x.IsDeleted = true);
            CurrentUnitOfWork.SaveChanges();
        }

        public async Task<MezonTokenDto> EditMezonToken(MezonTokenDto input)
        {
            var entity = await WorkScope.GetAll<MezonToken>().FirstOrDefaultAsync(x => x.Id == input.Id);
            entity.Amount = input.Amount;
            entity.EmployeeId = input.EmployeeId;
            entity.Note = input.Note;
            entity.SentToEmployeeAt = input.SentToEmployeeAt;
            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public List<ExportMezonTokenDto> GetFilterMezonToken(GridParam input)
        {
            var mezonTokens = GetAllPaging(input);
            return mezonTokens.Result.Items
                .Select(x => new ExportMezonTokenDto
                {
                    EmailAddress = x.Email,
                    Amount = x.Amount,
                })
                .ToList();
        }
        public async Task<FileBase64Dto> ExportMezonToken(GridParam input)
        {
            var templateFilePath = Path.Combine(HRMv2Consts.templateFolder, "Export-MezonToken.xlsx");

            if (templateFilePath == default)
            {
                throw new UserFriendlyException("Can't find template");
            }

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
                onboardMezonTokenSheet.Cells[onboardRowIndex, 1].Value = onboardRowIndex -1 ;
                onboardMezonTokenSheet.Cells[onboardRowIndex, 2].Value = mezonToken.EmailAddress;
                onboardMezonTokenSheet.Cells[onboardRowIndex, 3].Value = mezonToken.Amount;
                onboardMezonTokenSheet.Cells[onboardRowIndex, 3].Style.Numberformat.Format = "#,##0";
                onboardRowIndex++;
            }

        }
    }
}
