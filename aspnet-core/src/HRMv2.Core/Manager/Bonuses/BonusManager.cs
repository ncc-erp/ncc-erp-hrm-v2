using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Bonuses.Dto;
using HRMv2.Utils;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRMv2.Manager.Common.Dto;
using HRMv2.NccCore;
using HRMv2.Manager.Employees;
using Microsoft.AspNetCore.Mvc;
using static HRMv2.Constants.Enum.HRMEnum;
using System.IO;
using OfficeOpenXml;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Manager.Notifications.Email;
using Abp.BackgroundJobs;
using HRMv2.BackgroundJob.SendMail;

namespace HRMv2.Manager.Categories.Bonuss
{
    public class BonusManager : BaseManager
    { 
        private readonly EmployeeManager _employeeManager;
        private readonly EmailManager _emailManager;
        private readonly BackgroundJobManager _backgroundJobManager;
        public BonusManager(IWorkScope workScope, EmployeeManager employeeManager, EmailManager emailManager, BackgroundJobManager backgroundJobManager) : base(workScope)
        {
            _employeeManager = employeeManager;
            _emailManager = emailManager;
            _backgroundJobManager = backgroundJobManager;
        }

        public IQueryable<GetBonusDto> QueryAllBonus()
        {
            return WorkScope.GetAll<Bonus>().Select(x => new GetBonusDto
            {
                Id = x.Id,
                Name = x.Name,
                IsActive = x.IsActive,
                ApplyMonth = x.ApplyMonth
            });
        }

        public IQueryable<GetBonusEmployeeDto> QueryAllBonusEmployee()
        {
           return WorkScope.GetAll<BonusEmployee>()
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new GetBonusEmployeeDto
                {
                    Id = x.Id,
                    Money = x.Money,
                    Note = x.Note,
                    EmployeeId = x.EmployeeId,
                    BonusId = x.BonusId,
                    FullName = x.Employee.FullName,
                    LastModificationTime = x.LastModificationTime,
                    FullNameModification = x.CreatorUser.FullName,
                    Skills = x.Employee.EmployeeSkills.Select(s => new EmployeeSkillDto
                    {
                        SkillId = s.Skill.Id,
                        SkillName = s.Skill.Name
                    }).ToList(),
                    Teams = x.Employee.EmployeeTeams.Select(s => new EmployeeTeamDto
                    {
                        TeamId = s.Team.Id,
                        TeamName = s.Team.Name
                    }).ToList(),
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
                    JobPositionInfo = new BadgeInfoDto {
                        Name = x.Employee.JobPosition.Name,
                        Color = x.Employee.JobPosition.Color
                    },
                    JobPositionId = x.Employee.JobPositionId,
                    Avatar = x.Employee.Avatar,
                    Sex = x.Employee.Sex,
                    Email = x.Employee.Email,
                    BranchId = x.Employee.BranchId,
                    LevelId = x.Employee.LevelId,
                    UserType = x.Employee.UserType,
                    Status = x.Employee.Status,
                    BonusName = x.Bonus.Name,
                    ApplyDate = x.Bonus.ApplyMonth
                });
        }

        public async Task<GridResult<GetAllBonusDto>> GetAllPaging(GridParam input)
        {
            var queryBonusEmployee = await WorkScope.GetAll<BonusEmployee>()
               .GroupBy(x => x.BonusId)
               .Select(x => new { x.Key, EmployeeCount = x.Count(), TotalMoney = x.Sum(x => x.Money) })
               .ToDictionaryAsync(x => x.Key, x => new
               {
                   x.EmployeeCount,
                   x.TotalMoney
               });
            var listBonus = QueryAllBonus().OrderByDescending(x => x.ApplyMonth);
            var query = listBonus.Select(x => new GetAllBonusDto
            {
                TotalMoney = queryBonusEmployee.ContainsKey(x.Id) ? (double)(queryBonusEmployee[x.Id].TotalMoney) : 0,
                EmployeeCount = queryBonusEmployee.ContainsKey(x.Id) ? (int)(queryBonusEmployee[x.Id].EmployeeCount) : 0,
                Id = x.Id,
                Name = x.Name,
                ApplyMonth = x.ApplyMonth,
                IsActive = x.IsActive
            });
            return await query.GetGridResult(query, input);
        }

        public List<GetBonusDto> GetAll()
        {
            return QueryAllBonus().ToList();
        }

        public IQueryable<GetBonusesOfEmployeeDto> QueryBonusesByEmployeeId(long employeeId)
        {
            return WorkScope.GetAll<BonusEmployee>()
               .Where(x => x.EmployeeId == employeeId)
               .Select(x => new GetBonusesOfEmployeeDto
               {
                   Id = x.Id,
                   BonusId = x.BonusId,
                   BonusName = x.Bonus.Name,
                   ApplyMonth = x.Bonus.ApplyMonth,
                   IsActive = x.Bonus.IsActive,
                   Money = x.Money,
                   Note = x.Note,
                   UpdatedTime = x.LastModificationTime,
                   UpdatedUser = x.LastModifierUser != null ? x.LastModifierUser.FullName : "",
               });
        }


        public async Task<GridResult<GetBonusesOfEmployeeDto>> GetAllPagingBonusesByEmployeeId(long employeeId, GridParam input)
        {
            var query = QueryBonusesByEmployeeId(employeeId).OrderByDescending(x => x.ApplyMonth);
            return await query.GetGridResult(query, input);
        }

        public List<GetBonusesOfEmployeeDto> GetBonusesByEmployeeId(long id)
        {
            return QueryBonusesByEmployeeId(id).ToList();
        }

        public List<GetEmployeeBasicInfoDto> GetAllEmployeeNotInBonus(long id)
        {
            var employeeIdsInBonus = WorkScope.GetAll<BonusEmployee>()
                .Where(x => x.BonusId == id)
                .Select(x=> x.EmployeeId)
                .ToList();

            var query = _employeeManager.GetAllEmployeeBasicInfo()
                .Where(x => !employeeIdsInBonus.Contains(x.Id))
                .ToList();

            return query;
        }
        public async Task<BonusDto> Create(BonusDto input)
        {
            await ValidCreate(input);
            input.ApplyMonth = new DateTime(input.ApplyMonth.Year, input.ApplyMonth.Month, 15);
            input.IsActive = true;
            var entity = ObjectMapper.Map<Bonus>(input);
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);

            return input;
        }

        public async Task<EditBonusDto> Update(EditBonusDto input)
        {
            await CheckBonusIsActive(input.Id);
            //Update bonus
            await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<Bonus>(input.Id);

            if(input.ApplyMonth.Month != entity.ApplyMonth.Month)
            {
                await ValidUpdateMonth(input);
            }
            entity.Name = input.Name;
            entity.IsActive = input.IsActive;
            entity.ApplyMonth = new DateTime(input.ApplyMonth.Year, input.ApplyMonth.Month, 15);

            await WorkScope.UpdateAsync(entity);
            if (input.IsApply)
            {
                await UpdateBonusEmployee(input);
            }
            return input;
        }

        private async Task UpdateBonusEmployee(EditBonusDto input)
        {
            var listBonusEmployee = await WorkScope.GetAll<BonusEmployee>()
                                            .Where(x => x.BonusId == input.Id)
                                            .ToListAsync();

            foreach (var entity in listBonusEmployee)
            {
                entity.Note = input.Name;
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }


        public async Task<long> Delete(long id)
        {
            await ValidDelete(id);
            await WorkScope.DeleteAsync<Bonus>(id);

            return id;
        }

        private async Task ValidCreate(BonusDto input)
        {
            var isExist = await WorkScope.GetAll<Bonus>()
                .AnyAsync(x => x.Name.Trim() == input.Name.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"Name is Already Exist");
            }
        }

        private async Task ValidUpdate(EditBonusDto input)
        {
            var isExist = await WorkScope.GetAll<Bonus>()
                .AnyAsync(x => x.Id != input.Id && x.Name == input.Name);
            if (isExist)
            {
                throw new UserFriendlyException($"Name is Already Exist");
            }   
        }
        private async Task ValidUpdateMonth(EditBonusDto input)
        {
            var payroll = WorkScope.GetAll<Payroll>()
                .Where(x => x.Status == PayrollStatus.Executed)
                .OrderByDescending(x => x.ApplyMonth)
                .OrderByDescending(x => x.ApplyMonth)
                .FirstOrDefault();
            if (payroll != default && input.ApplyMonth <= payroll.ApplyMonth)
            {
                throw new UserFriendlyException($"Only update apply month > {payroll.ApplyMonth.ToString("MM/yyyy")}");
            }
        }
        private async Task ValidDelete(long id)
        {
            var entity = await WorkScope.GetAsync<Bonus>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no Bonus with id {id}");
            }
        }

        public async Task<bool> IsBonusHasEmployee(long bonusId)
        {

            var hadUser = await WorkScope.GetAll<BonusEmployee>()
                                .AnyAsync(x=> x.BonusId == bonusId);
            if (hadUser) return true;
            return false;
                
        }


        public List<DateTime> GetListDate()
        {
            var listDate = DateTimeUtils.GetListMonthsForCreate();
            return listDate;
        }


        public async Task<List<DateTime>> GetListMonthFilter()
        {
            var query = await WorkScope.GetAll<Bonus>()
                .Select(x => x.ApplyMonth)
                .Distinct()
                .OrderByDescending(x => x)
                .ToListAsync();

            return query;
        }

        public async Task<long> ChangeStatus(long id)
        {
            await ValidBonus(id);
            var entity = await WorkScope.GetAsync<Bonus>(id);
            entity.IsActive = !entity.IsActive;
            await WorkScope.UpdateAsync(entity);
            return id;
        }

        private async Task ValidBonus(long id)
        {
            var entity = await WorkScope.GetAsync<Bonus>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no Bonus with id {id}");
            }
        }

        public async Task<GetDetailBonusDto> GetBonusDetail(long id)
        {
            return await WorkScope.GetAll<Bonus>()
                .Where(x => x.Id == id)
                .Select(x => new GetDetailBonusDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive,
                    ApplyMonth = x.ApplyMonth,
                    CreationTime = x.CreationTime,
                    LastModificationTime = x.LastModificationTime,
                    FullNameCreation = x.CreatorUser.FullName,
                    FullNameModification = x.CreatorUser.FullName,
                }).FirstOrDefaultAsync();
        }

        public async Task<GridResult<GetBonusEmployeeDto>> GetAllBonusEmployee(long id, GetBonusEmployeeInputDto input)
        {
            var query = QueryAllBonusEmployee()
                .Where(x => x.BonusId == id);

            if (input.StatusIds != null && input.StatusIds.Count == 1) query = query.Where(x => input.StatusIds[0] == x.Status);
            else if (input.StatusIds != null && input.StatusIds.Count > 1) query = query.Where(x => input.StatusIds.Contains(x.Status));

            if (input.BranchIds != null && input.BranchIds.Count == 1) query = query.Where(x => input.BranchIds[0] == x.BranchId);
            else if (input.BranchIds != null && input.BranchIds.Count > 1) query = query.Where(x => input.BranchIds.Contains(x.BranchId));

            if (input.UserTypes != null && input.UserTypes.Count == 1) query = query.Where(x => input.UserTypes[0] == x.UserType);
            else if (input.UserTypes != null && input.UserTypes.Count > 1) query = query.Where(x => input.UserTypes.Contains(x.UserType));

            if (input.LevelIds != null && input.LevelIds.Count == 1) query = query.Where(x => input.LevelIds[0] == x.LevelId);
            else if (input.LevelIds != null && input.LevelIds.Count > 1) query = query.Where(x => input.LevelIds.Contains(x.LevelId));

            if (input.JobPositionIds != null && input.JobPositionIds.Count == 1) query = query.Where(x => input.JobPositionIds[0] == x.JobPositionId);
            else if (input.JobPositionIds != null && input.JobPositionIds.Count > 1) query = query.Where(x => input.JobPositionIds.Contains(x.JobPositionId));

            if (input.TeamIds == null || input.TeamIds.Count == 0)
            {
                // TODO: Testcase GetAllBonusEmployee_CheckPaging_SearchParam - Test search not right
                return await query.GetGridResult(query, input.GridParam);
            }

            if (input.TeamIds.Count == 1 || !input.IsAndCondition)
            {
                var employeeHaveAnyTeams = QueryEmployeeHaveAnyTeams(input.TeamIds).Distinct();

                query = from employee in query
                        join employeeId in employeeHaveAnyTeams on employee.EmployeeId equals employeeId
                        select employee;
                return await query.GetGridResult(query, input.GridParam);

            }

            var employeeIds = QueryEmployeeHaveAllTeams(input.TeamIds).Result;
            query = query.Where(s => employeeIds.Contains(s.EmployeeId));

            return await query.GetGridResult(query, input.GridParam);
        }



        private async Task ValidAddEmployee(long employeeId, long bonusId)
        {
            var isExist = await WorkScope.GetAll<BonusEmployee>()
                .Where(x => x.EmployeeId == employeeId)
                .Where(x => x.BonusId == bonusId).AnyAsync();
            if (isExist)
            {
                throw new UserFriendlyException($"This User Is Already Exist");
            }
        }
        private async Task<BonusEmployee> ExitEmployeeInBonus(long employeeId, long bonusId)
        {
            return await WorkScope.GetAll<BonusEmployee>()
                .Where(x => x.EmployeeId == employeeId)
                .Where(x => x.BonusId == bonusId).FirstOrDefaultAsync();
        }

        public async Task<AddEmployeeToBonusDto> QuickAddEmployeeToBonus(AddEmployeeToBonusDto input)
        {
            await ValidBonus(input.BonusId);
            await AddEmployeeToBonus(input);
            return input;
        }


        public async Task<AddBonusForEmployeeDto> AddBonusForEmployee(AddBonusForEmployeeDto input)
        {
            await CheckBonusAldreadyAddForEmployee(input);
            var entity = ObjectMapper.Map<BonusEmployee>(input);
            entity.Note = QueryAllBonus()
                          .Where(x => x.Id == input.BonusId)
                          .FirstOrDefault().Name;
            await WorkScope.InsertAsync(entity);
            return input;
        }

        private async Task CheckBonusAldreadyAddForEmployee(AddBonusForEmployeeDto input)
        {
            var isExist = await WorkScope.GetAll<BonusEmployee>()
                .AnyAsync(x => x.BonusId == input.BonusId && x.EmployeeId == input.EmployeeId);
            if (isExist)
            {
                throw new UserFriendlyException("Employee already has this bonus");
            }
        }

        public async Task<UpdateBonusEmployeeDto> UpdateBonusEmployee(UpdateBonusEmployeeDto input)
        {
            var entity = await WorkScope.GetAsync<BonusEmployee>(input.Id);
           
            ObjectMapper.Map(input, entity);

            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public async Task<long> RemoveBonusFromEmployee(long id)
        {
            await WorkScope.DeleteAsync<BonusEmployee>(id);
            return id;
        }

        public async Task<AddEmployeeToBonusDto> MultipleAddEmployeeToBonus(AddEmployeeToBonusDto input)
        {
            await AddEmployeeToBonus(input);
            return input;
        }

        private async Task AddEmployeeToBonus(AddEmployeeToBonusDto input)
        {
            await CheckBonusIsActive(input.BonusId);
            if (input.EmployeeIds.Count == 1)
            {
                await ValidAddEmployee(input.EmployeeIds.FirstOrDefault(), input.BonusId);
                var bonusEmployee = new BonusEmployee
                {
                    BonusId = input.BonusId,
                    EmployeeId = input.EmployeeIds.FirstOrDefault(),
                    Money = input.Money,
                    Note = input.Note
                };
                if (input.Note == null) bonusEmployee.Note = "1";
                var entity = ObjectMapper.Map<BonusEmployee>(bonusEmployee);
                await WorkScope.InsertAsync(entity);
            }
            else
            {
                foreach (var employeeId in input.EmployeeIds)
                {
                    var exitEmployeeInBonus = await ExitEmployeeInBonus(employeeId, input.BonusId);
                    if (exitEmployeeInBonus != default)
                    {
                        exitEmployeeInBonus.Note = input.Note;
                        exitEmployeeInBonus.Money = input.Money;
                        await WorkScope.UpdateAsync(exitEmployeeInBonus);
                    }
                    else
                    {
                        var bonusEmployee = new BonusEmployee
                        {
                            BonusId = input.BonusId,
                            EmployeeId = employeeId,
                            Money = input.Money,
                            Note = input.Note
                        };
                        var entity = ObjectMapper.Map<BonusEmployee>(bonusEmployee);
                        await WorkScope.InsertAsync(entity);
                    }

                }
            }
        }

        public async Task<EditEmployeeToBonusDto> UpdateEmployeeInBonus(EditEmployeeToBonusDto input)
        {
            await CheckBonusIsActive(input.BonusId);
            await CheckExitEmployeeInBonus(input.Id);
            var entity = await WorkScope.GetAsync<BonusEmployee>(input.Id);
            ObjectMapper.Map(input, entity);
            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public async Task<long> DeleteEmployeeFromBonus(long id, long bonusId)
        {
            await CheckBonusIsActive(bonusId);
            await CheckExitEmployeeInBonus(id);
            // TODO: Can xem xét, trường hợp truyền lên id đúng, bonusId đúng nhưng không phải là bonusId đang ở cùng 1 record với id thì vẫn xóa được?
            await WorkScope.DeleteAsync<BonusEmployee>(id);
            return id;
        }
        private async Task CheckExitEmployeeInBonus(long id)
        {
            var entity = await WorkScope.GetAsync<BonusEmployee>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no employee with id {id}");
            }
        }

        private async Task CheckBonusIsActive(long bonusId)
        {
            var entity = await WorkScope.GetAll<Bonus>()
                .Where(x => x.Id == bonusId)
                .Select(x => x.IsActive)
                .FirstOrDefaultAsync();
            if (entity == false)
            {
                throw new UserFriendlyException($"Bonus not active");
            }
        }

        public async Task<List<long>> GetAllEmployeeInBonus(long bonusId)
        {
            return await WorkScope.GetAll<BonusEmployee>()
                .Where(x => x.BonusId == bonusId)
                .Select(x => x.EmployeeId).ToListAsync();
        }

             public async Task<List<DateTime>> GetListMonthFilterOfEmployee(long employeeId)
        {
            var query = await WorkScope.GetAll<BonusEmployee>()
                .Where(x => x.EmployeeId == employeeId)
                .Select(x => x.Bonus.ApplyMonth)
                .Distinct()
                .OrderByDescending(x => x)
                .ToListAsync();

            return query;
        }

        public GetBonusDto GetBonusById(long id)
        {
            var query = QueryAllBonus()
                .Where(x => x.Id == id).FirstOrDefault();
            return query;
        }

        public async Task<Object> ImportEmployeeToBonus([FromForm] ImportFileDto input)
        {

            await ValidImportEmployeeToBonus(input);

            var successList = new List<BonusEmployee>();

            var failedList = new List<ResponseFailDto>();

            var employeeEmailSuccessList = new List<string>();

            var setEmployeeAlreadyExist = WorkScope.GetAll<BonusEmployee>()
                .Where(x => x.BonusId == input.BonusId)
                    .Select(x => x.Employee.Email.ToLower().Trim())
                    .Distinct()
                    .ToHashSet();

            var mapEmailToId = await WorkScope.GetAll<Employee>()
                .Select(x=> new {x.Email, x.Id})
                .ToDictionaryAsync(k=> k.Email, v=> v.Id);

            var bonusName = GetBonusById(input.BonusId).Name;

            using (var stream = new MemoryStream())
            {
                input.File.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    var columnCount = worksheet.Dimension.Columns;

                    if (columnCount < 2)
                    {
                        throw new UserFriendlyException("Number of columns < 2 => Invlid format");
                    }
                    var rowCount = worksheet.Dimension.End.Row;

                    BonusEmployee bonusEmployee = null;
   
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var email = worksheet.Cells[row, 1].GetCellValue<string>();
                        var strMoney = worksheet.Cells[row, 2].GetCellValue<string>();
                        try
                        {

                            if (string.IsNullOrEmpty(email))
                            {
                                failedList.Add(new ResponseFailDto { Row = row, Email = email, Money = strMoney, ReasonFail = "Email null or empty" });
                                continue;
                            }

                            email = email.Trim().ToLower();

                            if (!mapEmailToId.ContainsKey(email))
                            {
                                failedList.Add(new ResponseFailDto { Row = row, Email = email, Money = strMoney, ReasonFail = "Email not found" });
                                continue;
                            }
                            var employeeId = mapEmailToId[email.ToLower().Trim()];
                            var money = worksheet.Cells[row, 2].GetCellValue<long>();


                            if (setEmployeeAlreadyExist.Contains(email) || employeeEmailSuccessList.Contains(email))
                            {
                                failedList.Add(new ResponseFailDto { Row = row, Email = email, Money = strMoney, ReasonFail = "Already imported" });
                                continue;
                            }

                            bonusEmployee = new BonusEmployee
                            {
                                EmployeeId = employeeId,
                                Money = money,
                                Note = bonusName,
                                BonusId = input.BonusId,
                            };
                            employeeEmailSuccessList.Add(email);
                            successList.Add(bonusEmployee);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("row: " + row + ", error: " + ex.Message);
                            failedList.Add(new ResponseFailDto { Row = row, Email = email, Money = strMoney, ReasonFail = "Exception: " + ex.Message });
                        }
                        
                    }
                    await WorkScope.InsertRangeAsync(successList);
                }
            }
            return new { successList, failedList };



        }
        public async Task ValidImportEmployeeToBonus([FromForm] ImportFileDto input)
        {
            if(input.File == null || !Path.GetExtension(input.File.FileName).Equals(".xlsx"))
            {
                throw new UserFriendlyException("File upload is invalid");
            }
        }
        public MailPreviewInfoDto GetBonusTemplate(long bonusEmployeeId)
        {
            MailPreviewInfoDto template = _emailManager.GetEmailContentById(MailFuncEnum.Bonus, bonusEmployeeId);
            return template;
        }

        public void SendMailToOneEmployee(SendMailBonusDto input)
        {
            var bonusEmployee = WorkScope.GetAll<BonusEmployee>()
               .Where(x => x.Id == input.BonusEmployeeId)
               .FirstOrDefault();
            if (bonusEmployee == default)
            {
                throw new UserFriendlyException($"Can not found bonus employee with Id = {input.BonusEmployeeId}");
            }
            _emailManager.Send(input.MailContent);
        }

        public string SendMailToAllEmployee(long id, GetBonusEmployeeInputDto input)
        {
            var emailTemplate = _emailManager.GetEmailTemplateDto(MailFuncEnum.Bonus);
            if(emailTemplate == default)
            {
                throw new UserFriendlyException($"Not found email template for bonus");
            }
            var listBonusEmployees = GetAllBonusEmployee(id, input).Result.Items;

            List<ResultTemplateEmail<BonusMailTemplateDto>> emailBonusEmployees = listBonusEmployees.Select(x => new ResultTemplateEmail<BonusMailTemplateDto>
            {
                Result = new BonusMailTemplateDto
                {
                    EmployeeFullName = x.FullName,
                    SendToEmail = x.Email,
                    BonusMoney = CommonUtil.FormatDisplayMoney(x.Money),
                    BonusName = x.BonusName,
                    ApplyMonth = x.ApplyDate.ToString("MM/yyyy"),
                }
            }).ToList();

            var delaySendMail = 0;
            foreach(var be in emailBonusEmployees)
            {
                MailPreviewInfoDto mailInput = _emailManager.GenerateEmailContent(be.Result, emailTemplate);
                _backgroundJobManager.Enqueue<SendMail, MailPreviewInfoDto>(mailInput, BackgroundJobPriority.High, TimeSpan.FromSeconds(delaySendMail));
                delaySendMail += HRMv2Consts.DELAY_SEND_MAIL_SECOND;
            }
  

            return $"Started sending {emailBonusEmployees.Count} email.";

        }



    }
}
