using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.Employees;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Punishments.Dto;
using HRMv2.NccCore;
using HRMv2.WebServices.Timesheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using NccCore.Uitls;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.Punishments
{
    public class PunishmentManager : BaseManager
    {
        private readonly EmployeeManager _employeeManager;
        public PunishmentManager(IWorkScope workScope , EmployeeManager employeeManager) : base(workScope)
        {
            _employeeManager = employeeManager;
        }

        public IQueryable<PunishmentDto> QueryAllPunishment()
        {
            return WorkScope.GetAll<Punishment>()
                .OrderByDescending(s => s.Date)
                .ThenByDescending(s => s.CreationTime)
                .Select(x => new PunishmentDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Date = x.Date,
                    IsActive = x.IsActive,
                    PunishmentTypeId = x.PunishmentTypeId
                });
        }

        public async Task<GridResult<GetPunishmentDto>> GetAllPaging(GridParam input)
        {
            var listPuns = QueryAllPunishment();
            var queryPunishmentEmployee = await WorkScope.GetAll<PunishmentEmployee>()
               .GroupBy(x => x.PunishmentId)
               .Select(x => new { x.Key, EmployeeCount = x.Count(), TotalMoney = x.Sum(x => x.Money) })
               .ToDictionaryAsync(x => x.Key, x => new
               {
                   x.EmployeeCount,
                   x.TotalMoney 
               });
            var query = listPuns.Select(x => new GetPunishmentDto
            {
                TotalMoney = queryPunishmentEmployee.ContainsKey(x.Id) ? (double)(queryPunishmentEmployee[x.Id].TotalMoney) : 0,
                EmployeeCount = queryPunishmentEmployee.ContainsKey(x.Id) ? (int)(queryPunishmentEmployee[x.Id].EmployeeCount) : 0,
                Id = x.Id,
                Name = x.Name,
                Date = x.Date,
                IsActive = x.IsActive
            });
            return await query.GetGridResult(query, input);
        }

        public List<PunishmentDto> GetAll()
        {
            return QueryAllPunishment().ToList();
        }
        public async Task<GridResult<GetPunishmentsOfEmployeeDto>> GetPunishmentByEmployeeId(long id, GridParam input)
        {
            var query = WorkScope.GetAll<PunishmentEmployee>()
                .Where(x => x.EmployeeId == id)
                .Select(x => new GetPunishmentsOfEmployeeDto
                {
                    Id = x.Id,
                    PunishmentId = x.PunishmentId,
                    PunishmentName = x.Punishment.Name,
                    ApplyMonth = x.Punishment.Date,
                    IsActive = x.Punishment.IsActive,
                    Money = x.Money,
                    Note = x.Note,
                    UpdatedTime = x.LastModificationTime,
                    UpdatedUser = x.LastModifierUser != null ? x.LastModifierUser.FullName : "",
                });
            return await query.GetGridResult(query, input);
        }

        public async Task<CreatePunishmentDto> Create(CreatePunishmentDto input)
        {
            if (!ValidCreate(input.Name))
            {
                throw new UserFriendlyException("Name is already exist");
            };
            var entity = ObjectMapper.Map<Punishment>(input);
            entity.IsActive = true;
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);
            return input;

        }

        public async Task<List<ResultGeneratePunishmentDto>> GeneratePunishmentsByPunishmentType(GeneratePunishmentDto input)
        {

            var punishmentTypes = WorkScope.GetAll<PunishmentType>()
                .Where(s => s.IsActive)
                .Where(s => input.PunishmentTypeIds.Contains(s.Id))
                .Select(s => new { s.Id, s.Name, s.Api })
                .ToList();

            var dicEmployeeEmailToId = GetDicEmployeeEmailToId();

            var results = new List<ResultGeneratePunishmentDto>();

            foreach (var punishmentType in punishmentTypes)
            {
                try
                {
                    var resultItem = await GeneratePunishment(punishmentType.Id, punishmentType.Name, punishmentType.Api, input.Date, dicEmployeeEmailToId);
                    results.Add(resultItem);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    results.Add(new ResultGeneratePunishmentDto { PunishmentTypeId = punishmentType.Id, Message = ex.Message});
                }
            }

            return results;
        }


        private async Task<ResultGeneratePunishmentDto> GeneratePunishment(long punishmentTypeId, string punishmentTypeName, string api, DateTime date, Dictionary<string, long> dicEmployeeEmailToId)
        {
            var name = punishmentTypeName + " " + DateTimeUtils.ToMMYYYY(date);
            if (!ValidCreate(name))
            {
                return new ResultGeneratePunishmentDto { PunishmentTypeId = punishmentTypeId, Message = name + ": already exist" };
            }

            var responseObj = GetPunishmentFromApi(api, date.Year, date.Month);


            if (responseObj == default || responseObj.Result == null)
            {
                return new ResultGeneratePunishmentDto { PunishmentTypeId = punishmentTypeId, Message = responseObj?.Error };
            }

            if (responseObj.Result.IsEmpty())
            {
                return new ResultGeneratePunishmentDto { PunishmentTypeId = punishmentTypeId, Message = "Lấy từ api: 0" };
            }

            var emailMoneyList = responseObj.Result.Where(x=>x.Email !=null).ToList();
            var allEmployeeEmail = dicEmployeeEmailToId.Keys.ToList();

            var insertEmailMoneyList = emailMoneyList.Where(s => dicEmployeeEmailToId.ContainsKey(s.Email));
            if (insertEmailMoneyList.IsEmpty())
            {
                return new ResultGeneratePunishmentDto { PunishmentTypeId = punishmentTypeId, Message = $"Lấy từ api: {emailMoneyList.Count}. Không email nào tồn tại trong hệ thống" };
            }

            var punishmentId = await CreatePunishment(punishmentTypeId, name, date);


            var listPunishmentEmployee = insertEmailMoneyList.Select(item => new PunishmentEmployee
            {
                PunishmentId = punishmentId,
                Money = item.Money,
                Note = name,
                EmployeeId = dicEmployeeEmailToId[item.Email]
            }).ToList();

            WorkScope.InsertRangeAsync(listPunishmentEmployee);

            return new ResultGeneratePunishmentDto { PunishmentTypeId = punishmentTypeId, Message = "Inserting to DB" };

        }


        private ResponseApiPunishmentDto GetPunishmentFromApi(string api, int year, int month)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(api);
                var url = "?year=" + year + "&month=" + month;
                var fullUrl = $"{httpClient.BaseAddress}/{url}";
                try
                {
                    Logger.Info($"Get: {fullUrl}");
                    var response = httpClient.GetAsync(url).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return new ResponseApiPunishmentDto { Error = response.ReasonPhrase +" url"};
                        
                    }
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    Logger.Info($"Get: {fullUrl} response: {responseContent}");
                    var responseObj = JsonConvert.DeserializeObject<ResponseApiPunishmentDto>(responseContent);
                    return responseObj;



                }
                catch (Exception ex)
                {
                    Logger.Error($"Get: {fullUrl} error: {ex.Message}");
                    return new ResponseApiPunishmentDto { Error = ex.Message };
                }
                return default;
            }
        }

        private async Task<long> CreatePunishment(long punishmentTypeId, string name, DateTime date)
        {
            var entity = new Punishment
            {
                Date = date,
                IsActive = true,
                Name = name,
                PunishmentTypeId = punishmentTypeId
            };

            entity.Id = await WorkScope.InsertAndGetIdAsync(entity);
            return entity.Id;

        }


        public async Task<UpdatePunishmentDto> Update(UpdatePunishmentDto input)
        {
            await ValidUpdate(input);
            var entity = await WorkScope.GetAsync<Punishment>(input.Id);
            ObjectMapper.Map(input, entity);
            await WorkScope.UpdateAsync(entity);
            if (input.isAbleUpdateNote == true)
            {
                var query = WorkScope.GetAll<PunishmentEmployee>()
                    .Where(x => x.PunishmentId == input.Id).ToList();
                query.ForEach(x => x.Note = input.Name);
                await WorkScope.UpdateRangeAsync(query);
            }

            return input;
        }

        private async Task ValidUpdate(UpdatePunishmentDto input)
        {
            var isExist = await WorkScope.GetAll<Punishment>()
                .AnyAsync(x => x.Id != input.Id && x.Name.Trim() == input.Name.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"This Punishment is Already Exist");
            }
        }

        private bool ValidCreate(string name)
        {
            var isExist = WorkScope.GetAll<Punishment>()
               .Any(x => x.Name.Trim() == name.Trim());

            return !isExist;
        }

        public async Task<long> Delete(long id)
        {
            await ValidDelete(id);
            await WorkScope.DeleteAsync<Punishment>(id);
            return id;
        }

        public async Task<long> ChangeStatus(long id)
        {
            var entity = await WorkScope.GetAsync<Punishment>(id);
            entity.IsActive = !entity.IsActive;
            await WorkScope.UpdateAsync(entity);
            return id;
        }
        private async Task ValidDelete(long id)
        {
            var entity = await WorkScope.GetAsync<Punishment>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no punishment with id {id}");
            }
        }

        public async Task<bool> IsPunishmentHasEmployee(long punishId)
        {
            var hadUsers = await WorkScope.GetAll<PunishmentEmployee>().AnyAsync(s => s.PunishmentId == punishId);
            if (hadUsers) return true;
            return false;
        }

        public PunishmentDto GetPunishmentById(long id)
        {
            var query = QueryAllPunishment()
                .Where(x => x.Id == id).FirstOrDefault();
            return query;
        }

        public async Task<AddEmployeeToPunishmentDto> AddEmployeeToPunishment(AddEmployeeToPunishmentDto input)
        {
            await ValidAddEmployee(input);         
            var entity = ObjectMapper.Map<PunishmentEmployee>(input);
            entity.Note = !input.Note.IsNullOrWhiteSpace() ? input.Note : QueryAllPunishment()
                .Where(x => x.Id == input.PunishmentId).FirstOrDefault().Name;
            input.Id = await WorkScope.InsertAndGetIdAsync(entity);
            return input;
        }

        private async Task ValidAddEmployee(AddEmployeeToPunishmentDto input)
        {
            var isExist = await WorkScope.GetAll<PunishmentEmployee>()
                .AnyAsync(x => x.PunishmentId == input.PunishmentId && x.EmployeeId == input.EmployeeId);
            if (isExist)
            {
                throw new UserFriendlyException($"This User Is Already Exist");
            }
        }
        public async Task<UpdateEmployeeInPunishmentDto> UpdateEmployeeInPunishment(UpdateEmployeeInPunishmentDto input)
        {
            await ValidUpdateEmployee(input);
            return await UpdatePunishmentEmployee(input);
        }

        private async Task<UpdateEmployeeInPunishmentDto> UpdatePunishmentEmployee(UpdateEmployeeInPunishmentDto input)
        {       
            var entity = await WorkScope.GetAsync<PunishmentEmployee>(input.Id);
            ObjectMapper.Map(input, entity);
            await WorkScope.UpdateAsync(entity);
            return input;
        }

        public async Task<UpdateEmployeeInPunishmentDto> UpdatePunishmentOfEmployee(UpdateEmployeeInPunishmentDto input)
        {
            await ValidUpdatePunishmentOfEmployee(input);
            return await UpdatePunishmentEmployee(input);
        }

        public async Task ValidUpdatePunishmentOfEmployee(UpdateEmployeeInPunishmentDto input)
        {
            var data = await WorkScope.GetAll<PunishmentEmployee>().ToListAsync();
            var isExist = await WorkScope.GetAll<PunishmentEmployee>()
                .AnyAsync(x => x.Id != input.Id && x.PunishmentId == input.PunishmentId);
            if (isExist)
            {
                throw new UserFriendlyException($"This User Is Already Exist");
            }
        }
        private async Task ValidUpdateEmployee(UpdateEmployeeInPunishmentDto input)
        {
            var data = await WorkScope.GetAll<PunishmentEmployee>().ToListAsync();
            var isExist = await WorkScope.GetAll<PunishmentEmployee>()
                .AnyAsync(x => x.Id != input.Id && x.PunishmentId == input.PunishmentId && x.EmployeeId == input.EmployeeId);
            if (isExist)
            {
                throw new UserFriendlyException($"This User Is Already Exist");
            }
        }

        public async Task<long> DeleteEmployeeFromPunishment(long id)
        {
            await ValidDeleteEmployee(id);
            await WorkScope.DeleteAsync<PunishmentEmployee>(id);
            return id;
        }
        private async Task ValidDeleteEmployee(long id)
        {
            var entity = await WorkScope.GetAsync<PunishmentEmployee>(id);
            if (entity == null)
            {
                throw new UserFriendlyException($"There is no employee with id {id}");
            }
        }

        public async Task<GridResult<GetPunishmentDetailDto>> GetAllEmployeeInPunishment(long id, GetEmployeePunishment input)
        {
            var query = WorkScope.GetAll<PunishmentEmployee>()
                .Where(x => x.PunishmentId == id)
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new GetPunishmentDetailDto
                {
                    Id = x.Id,
                    Money = x.Money,
                    Note = x.Note,
                    EmployeeId = x.EmployeeId,
                    PunishmentId = x.PunishmentId,
                    UpdatedTime = x.LastModificationTime,
                    UpdatedUser = x.LastModifierUser != null ? x.LastModifierUser.FullName : "",
                    FullName = x.Employee.FullName,
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
                    JobPositionId = x.Employee.JobPositionId,
                    Avatar = x.Employee.Avatar,
                    Sex = x.Employee.Sex,
                    Email = x.Employee.Email,
                    BranchId = x.Employee.BranchId,
                    LevelId = x.Employee.LevelId,
                    UserType = x.Employee.UserType,
                    Status = x.Employee.Status,
                });

            if (input.StatusIds != null && input.StatusIds.Count == 1) query = query.Where(x => input.StatusIds[0] == x.Status);
            else if (input.StatusIds != null && input.StatusIds.Count > 1) query = query.Where(x => input.StatusIds.Contains(x.Status));

            if (input.BranchIds != null && input.BranchIds.Count == 1) query = query.Where(x => input.BranchIds[0] == x.BranchId);
            else if (input.BranchIds != null && input.BranchIds.Count > 1) query = query.Where(x => input.BranchIds.Contains(x.BranchId));

            if (input.Usertypes != null && input.Usertypes.Count == 1) query = query.Where(x => input.Usertypes[0] == x.UserType);
            else if (input.Usertypes != null && input.Usertypes.Count > 1) query = query.Where(x => input.Usertypes.Contains(x.UserType));

            if (input.LevelIds != null && input.LevelIds.Count == 1) query = query.Where(x => input.LevelIds[0] == x.LevelId);
            else if (input.LevelIds != null && input.LevelIds.Count > 0) query = query.Where(x => input.LevelIds.Contains(x.LevelId));

            if (input.JobPositionIds != null && input.JobPositionIds.Count == 1) query = query.Where(x => input.JobPositionIds[0] == x.JobPositionId);
            else if (input.JobPositionIds != null && input.JobPositionIds.Count > 0) query = query.Where(x => input.JobPositionIds.Contains(x.JobPositionId));

            if (input.TeamIds == null || input.TeamIds.Count == 0)
            {
                return await query.GetGridResult(query, input.GridParam);
            }

            if (input.TeamIds.Count == 1 || !input.IsAndCondition)
            {
                var employeeHaveAnyTeams = _employeeManager.QueryEmployeeHaveAnyTeams(input.TeamIds).Distinct();

                query = from employee in query
                               join employeeId in employeeHaveAnyTeams on employee.EmployeeId equals employeeId
                               select employee;
                return await query.GetGridResult(query, input.GridParam);

            }

            var employeeIds = _employeeManager.QueryEmployeeHaveAllTeams(input.TeamIds).Result;

            query = query.Where(s => employeeIds.Contains(s.EmployeeId));

            return await query.GetGridResult(query, input.GridParam);

        }

        public List<GetEmployeeBasicInfoDto> GetAllEmployeeNotInPunishment(long id)
        {
            var employeeIdsInPunishment = WorkScope.GetAll<PunishmentEmployee>()
                                         .Where(p => p.PunishmentId == id)
                                         .Select(p=> p.EmployeeId).ToList();    
            var query = _employeeManager.GetAllEmployeeBasicInfo()
                .Where(p => !employeeIdsInPunishment.Contains(p.Id))
                .ToList();
            return query;
        }

        public List<DateTime> GetListDate()
        {
            var listDate = DateTimeUtils.GetListMonthsForCreate();
            return listDate;
        }

        public List<DateTime> GetDateFromPunishments()
        {
            var query = QueryAllPunishment().Select(x => x.Date).Distinct().OrderByDescending(x => x.Date).ToList();
            return query;
        }

        public List<DateTime> GetDateFromPunishmentsOfEmployee(long id)
        {
            return WorkScope.GetAll<PunishmentEmployee>()
                .Where(x => x.EmployeeId == id)
                .Select(x => x.Punishment.Date)
                .OrderByDescending(x => x.Date)
                .Distinct()
                .ToList();
        }

        public Object ImportEmployeePunishmentsFromFile([FromForm] ImportFileDto input)
        {
            ValidImportEmployeeToPunishment(input);

            var successList = new List<PunishmentEmployee>();

            var failedList = new List<ResponseFailDto>();

            var employeeEmailSuccessList = new List<string>();

            var setEmployeeAlreadyExist = WorkScope.GetAll<PunishmentEmployee>()
                    .Where(x => x.PunishmentId == input.PunishmentId)
                    .Select(x => x.Employee.Email.ToLower().Trim())
                    .Distinct()
                    .ToHashSet();

            var mapEmailToId = WorkScope.GetAll<Employee>()
                .Select(s => new {Email = s.Email.ToLower().Trim(), s.Id })
                .ToDictionary(s => s.Email, k => k.Id);

            var punishment = GetPunishmentById(input.PunishmentId);

            if (punishment == default)
            {
                throw new UserFriendlyException("Not found Punishment Id " + input.PunishmentId);
            }

            var punishmentNote = punishment.Name;

            using (var stream = new MemoryStream())
            {
                input.File.CopyTo(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    var columnCount = worksheet.Dimension.Columns;

                    if (columnCount < 3)
                    {
                        throw new UserFriendlyException("Number of columns < 3 => Invlid format");
                    }
                    var rowCount = worksheet.Dimension.End.Row;
                    PunishmentEmployee userPunishment = null;
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var email = worksheet.Cells[row, 1].GetCellValue<string>();
                        var strMoney = worksheet.Cells[row, 2].GetCellValue<string>();

                        try
                        {
                            
                            if (string.IsNullOrEmpty(email)){
                                failedList.Add(new ResponseFailDto { Row = row, Email = email, Money = strMoney, ReasonFail = "Email null or empty" });
                                continue;
                            }

                            email = email.Trim().ToLower();

                            if (!mapEmailToId.ContainsKey(email))
                            {
                                failedList.Add(new ResponseFailDto { Row = row, Email = email, Money = strMoney, ReasonFail = "Email not found"});
                                continue;
                            }
                            var employeeId = mapEmailToId[email.ToLower().Trim()];
                            var money = worksheet.Cells[row, 2].GetCellValue<long>();
                            var note = worksheet.Cells[row, 3].GetCellValue<string>();
                                                       

                            if (setEmployeeAlreadyExist.Contains(email) || employeeEmailSuccessList.Contains(email))
                            {
                                failedList.Add(new ResponseFailDto { Row = row, Email = email, Money = strMoney, ReasonFail = "Already imported" });
                                continue;
                            }

                            userPunishment = new PunishmentEmployee
                            {
                                EmployeeId = employeeId,
                                Money = money,
                                Note = String.IsNullOrEmpty(note) ? punishmentNote : note,
                                PunishmentId = input.PunishmentId,
                            };
                            employeeEmailSuccessList.Add(email);
                            successList.Add(userPunishment);
                        }
                        catch(Exception ex)
                        {
                            Logger.Error("row: " + row + ", error: " + ex.Message);
                            failedList.Add(new ResponseFailDto { Row = row, Email = email, Money = strMoney, ReasonFail = "Exception: " + ex.Message });
                        }
                        
                    }

                    WorkScope.InsertRangeAsync(successList);
                }
            }
            return new { successList, failedList };
        }

        // TODO: need to change && to || to run unit test method ImportEmployeePunishmentsFromFile_Test2 
        private void ValidImportEmployeeToPunishment([FromForm] ImportFileDto input)
        {
            if (input.File == null && !Path.GetExtension(input.File.FileName).Equals(".xlsx"))
            {
                throw new UserFriendlyException("File null or is not .xlsx file");
            }
        }
    }
    
}
