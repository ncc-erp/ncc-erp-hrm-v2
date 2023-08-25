using Abp.BackgroundJobs;
using Abp.Runtime.Caching;
using Abp.UI;
using HRMv2.BackgroundJob.SendMail;
using HRMv2.Entities;
using HRMv2.Manager.Categories.JobPositions;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.Levels.Dto;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.EmployeeContracts;
using HRMv2.Manager.Employees.Dto;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Manager.SalaryRequestEmployees.Dto;
using HRMv2.Manager.SalaryRequests.Dto;
using HRMv2.NccCore;
using HRMv2.Net.MimeTypes;
using HRMv2.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.SalaryRequests
{
    public class SalaryRequestManager : BaseManager
    {
        public readonly LevelManager _levelManager;
        public readonly JobPositionManager _jobPositionManager;
        public readonly ContractManager _contractManager;
        private readonly EmailManager _emailManager;
        private readonly BackgroundJobManager _backgroundJobManager;
        private readonly string templateFolder = Path.Combine("wwwroot", "template");


        public SalaryRequestManager(LevelManager levelManager,
            JobPositionManager jobPositionManager,
            ContractManager contractManager, IWorkScope workScope,
            EmailManager emailManager, BackgroundJobManager backgroundJobManager
            ) : base(workScope)
        {
            _levelManager = levelManager;
            _jobPositionManager = jobPositionManager;
            _contractManager = contractManager;
            _emailManager = emailManager;
            _backgroundJobManager = backgroundJobManager;
        }
        public IQueryable<GetSalaryRequestDto> QueryAllSalaryRequest()
        {
            return WorkScope.GetAll<SalaryChangeRequest>()
                .OrderByDescending(s => s.CreationTime)
                .Select(x => new GetSalaryRequestDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ApplyMonth = x.ApplyMonth,
                    Status = x.Status,
                    CreationTime = x.CreationTime,
                    CreatorUser = x.CreatorUser.FullName,
                    LastModifyUser = x.LastModifierUser.FullName,
                    LastModifyTime = (DateTime)x.LastModificationTime
                });
        }

        public async Task<GridResult<GetSalaryRequestDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllSalaryRequest();
            return await query.GetGridResult(query, input);
        }

        public List<GetSalaryRequestDto> GetAll()
        {
            return QueryAllSalaryRequest().ToList();
        }

        public GetSalaryRequestDto Get(long id)
        {
            return QueryAllSalaryRequest()
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public CreateSalaryRequestDto Create(CreateSalaryRequestDto input)
        {
            ValidCreate(input);

            var entity = ObjectMapper.Map<SalaryChangeRequest>(input);
            entity.Status = SalaryRequestStatus.New;

            WorkScope.InsertAsync(entity);
            return input;
        }

        public async Task<UpdateSalaryRequestDto> Update(UpdateSalaryRequestDto input)
        {
            ValidUpdate(input);

            var entity = ObjectMapper.Map<SalaryChangeRequest>(input);

            await WorkScope.UpdateAsync(entity);

            return input;
        }

        public long Delete(long id)
        {
            var request = WorkScope.GetAll<SalaryChangeRequest>()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if(request == default)
            {
                throw new UserFriendlyException($"Can't find request with id {id}");
            }

            var deleteRqEmployees = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .Where(x => x.SalaryChangeRequestId == id)
                .ToList();

            var deleteRqEmployeeIds = deleteRqEmployees.Select(x => x.Id).ToList();

            var deleteContracts = WorkScope.GetAll<EmployeeContract>()
                .Where(x => deleteRqEmployeeIds.Contains(x.SalaryRequestEmployeeId))
                .ToList();

            request.IsDeleted = true;

            foreach (var contract in deleteContracts)
            {
                contract.IsDeleted = true;
            }

            foreach (var re in deleteRqEmployees)
            {
                re.IsDeleted = true;
            }

            CurrentUnitOfWork.SaveChanges();

            return id;
        }

        public List<DateTime> GetListDateFromSalaryRequest()
        {
            var query = QueryAllSalaryRequest().Select(x => x.ApplyMonth)
                .Distinct()
                .OrderByDescending(x => x.Date)
                .ToList();
            return query;
        }

        public List<GetEmployeeNotInRequestDto> GetEmployeeNotInRequest(long requestId)
        {
            var employeeInRequest = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .Where(x => x.SalaryChangeRequestId == requestId)
                .Select(x => x.EmployeeId);

            return WorkScope.GetAll<Employee>()
                .Where(x => !employeeInRequest.Contains(x.Id))
                .Select(x => new GetEmployeeNotInRequestDto
                {
                    EmployeeId = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                })
                .ToList();
        }

        public IQueryable<GetRequestEmployeeDto> QueryAllRequestEmployee()
        {
            var listLevel = _levelManager.QueryAllLevel()
                .ToDictionary(x => x.Id, x => x.Name);

            var listJobPosition = _jobPositionManager.QueryAllJobPosition()
                .ToDictionary(x => x.Id, x => x.Name);

            return WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .OrderByDescending(s => s.CreationTime)
                .Select(x => new GetRequestEmployeeDto
                {
                    Id = x.Id,
                    FullName = x.Employee.FullName,
                    Avatar = x.Employee.Avatar,
                    Email = x.Employee.Email,
                    Sex = x.Employee.Sex,
                    EmployeeId = x.EmployeeId,
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
                    BranchInfo = new BadgeInfoDto
                    {
                        Name = x.Employee.Branch.Name,
                        Color = x.Employee.Branch.Color
                    },
                    //ContractPeriod = x.Employee.UserType.ContractPeriod,
                    SalaryChangeRequestId = x.SalaryChangeRequestId,
                    ApplyDate = x.ApplyDate,
                    Salary = x.Salary,
                    ToSalary = x.ToSalary,
                    FromLevelId = x.LevelId,
                    LevelName = listLevel.ContainsKey(x.LevelId) ? listLevel[x.LevelId] : null,
                    ToLevelId = x.ToLevelId,
                    ToLevelName = listLevel.ContainsKey(x.ToLevelId) ? listLevel[x.ToLevelId] : null,
                    FromUserType = x.FromUserType,
                    ToUserType = x.ToUserType,
                    FromJobPositionId = x.JobPositionId,
                    JobPositionName = listJobPosition.ContainsKey(x.JobPositionId) ? listJobPosition[x.JobPositionId] : null,
                    ToJobPositionId = x.ToJobPositionId,
                    ToJobPositionName = listJobPosition.ContainsKey(x.ToJobPositionId) ? listJobPosition[x.ToJobPositionId] : null,
                    Type = x.Type,
                    Note = x.Note,
                    UpdatedTime = (DateTime)x.LastModificationTime,
                    UpdatedUser = x.LastModifierUser.FullName != default ? x.LastModifierUser.FullName : "Ts tool" ,
                    HasContract = x.HasContract,
                    UserType = x.Employee.UserType,
                    BranchId = x.Employee.BranchId,
                    SalaryChangeRequestName = x.SalaryChangeRequest != null ? x.SalaryChangeRequest.Name : "",
                    ApplyMonth = x.SalaryChangeRequest != null ? x.SalaryChangeRequest.ApplyMonth : default
                });
        }

        public GetRequestEmployeeDto GetRequestEmployeeById(long id)
        {
            return QueryAllRequestEmployee()
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public async Task<long> AddEmployeeTosalaryRequest(AddOrUpdateEmployeeRequestDto input)
        {
            ValidAddEmployeeToSalaryRequest(input);
            var entity = ObjectMapper.Map<SalaryChangeRequestEmployee>(input);

            var initSalaryRequest = QueryAllRequestEmployee()
                .Where(x => x.EmployeeId == input.EmployeeId)
                .Where(x => x.Type == SalaryRequestType.Initial)
                .FirstOrDefault();

            if (initSalaryRequest == default)
            {
                entity.Type = SalaryRequestType.Initial;
                entity.SalaryChangeRequestId = null;
            }
            else
            {
                entity.Type = SalaryRequestType.Change;
            }

            input.Id = await WorkScope.InsertAndGetIdAsync(entity);

            if (input.HasContract)
            {
                _contractManager.CreateContractBySalaryRequest(input);
            }

            return input.Id;
        }

        public void ValidAddEmployeeToSalaryRequest(AddOrUpdateEmployeeRequestDto input)
        {
            var query = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .Where(x => x.EmployeeId == input.EmployeeId && x.SalaryChangeRequestId == input.SalaryChangeRequestId)
                .FirstOrDefault();
            if (query != null)
            {
                throw new UserFriendlyException($"Employee with Id= {input.EmployeeId} is already in this salary change request");
            }
        }

        public async Task<AddOrUpdateEmployeeRequestDto> UpdateSalaryRequestEmployee(AddOrUpdateEmployeeRequestDto input)
        {
            var entity = ObjectMapper.Map<SalaryChangeRequestEmployee>(input);
            await WorkScope.UpdateAsync(entity);

            var contract = WorkScope.GetAll<EmployeeContract>()
                .Where(x => x.SalaryRequestEmployeeId == input.Id)
                .FirstOrDefault();

            if (input.HasContract)
            {
                if (contract == default)
                {
                    _contractManager.CreateContractBySalaryRequest(input);
                }
                else
                {
                    await _contractManager.Update(input, contract);
                }
            }
            else if(contract != default)
            {
                await WorkScope.DeleteAsync(contract);
                entity.HasContract = false;
            }

            await WorkScope.UpdateAsync(entity);

            return input;
        }

        public async Task UpdateRequestStatus(UpdateChangeRequestDto input)
        {
            var request = WorkScope.GetAll<SalaryChangeRequest>()
                .Where(x => x.Id == input.RequestId)
                .FirstOrDefault();

            request.Status = input.Status;
            await WorkScope.UpdateAsync(request);

            if (input.Status == SalaryRequestStatus.Executed)
            {
                var employeesInRequest = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                    .Where(x => x.SalaryChangeRequestId == request.Id)
                    .ToList();

                foreach (var employee in employeesInRequest)
                {
                    var entity = WorkScope.GetAll<Employee>()
                        .Where(x => x.Id == employee.EmployeeId)
                        .FirstOrDefault();

                    if (entity != default)
                    {
                        entity.RealSalary = employee.ToSalary;
                        entity.UserType = employee.ToUserType;
                        entity.LevelId = employee.ToLevelId;
                        entity.JobPositionId = employee.ToJobPositionId;
                        if(employee.FromUserType != UserType.Staff && employee.ToUserType == UserType.Staff) entity.StartWorkingDate = employee.ApplyDate;
                        await WorkScope.UpdateAsync(entity);
                    }
                }
            }
        }
        public async Task DeleteSalaryRequestEmployee(long id)
        {
            await WorkScope.DeleteAsync<SalaryChangeRequestEmployee>(id);

            var contract = WorkScope.GetAll<EmployeeContract>()
            .Where(x => x.SalaryRequestEmployeeId == id)
            .FirstOrDefault();

            if ( contract != default)
            {
                await WorkScope.DeleteAsync(contract);
            }
        }

        public async Task<GridResult<GetRequestEmployeeDto>> GetEmployeesInSalaryRequest(long requestId, InputGetEmployeeInSalaryRequestDto input)
        {
            var query = QueryAllRequestEmployee()
                .Where(x => x.SalaryChangeRequestId == requestId)
                .Where(x => x.Type != SalaryRequestType.Initial);

            if (input.ToUsertypes != null && input.ToUsertypes.Count == 1) query = query.Where(x => input.ToUsertypes[0] == x.ToUserType);
            else if (input.ToUsertypes != null && input.ToUsertypes.Count > 1) query = query.Where(x => input.ToUsertypes.Contains(x.ToUserType));

            if (input.ToLevelIds != null && input.ToLevelIds.Count == 1) query = query.Where(x => input.ToLevelIds[0] == x.ToLevelId);
            else if (input.ToLevelIds != null && input.ToLevelIds.Count > 0) query = query.Where(x => input.ToLevelIds.Contains(x.ToLevelId));

            if (input.ToJobPositionIds != null && input.ToJobPositionIds.Count == 1) query = query.Where(x => input.ToJobPositionIds[0] == x.ToJobPositionId);
            else if (input.ToJobPositionIds != null && input.ToJobPositionIds.Count > 0) query = query.Where(x => input.ToJobPositionIds.Contains(x.ToJobPositionId));
            if (input.BranchIds != null && input.BranchIds.Count == 1) query = query.Where(x => input.BranchIds[0] == x.BranchId);
            else if (input.BranchIds != null && input.BranchIds.Count > 1) query = query.Where(x => input.BranchIds.Contains(x.BranchId));
            return await query.GetGridResult(query, input.GridParam);
        }


        private void ValidUpdate(UpdateSalaryRequestDto input)
        {
            var isExist = WorkScope.GetAll<SalaryChangeRequest>()
                .Any(x => x.Id != input.Id && x.Name.Trim() == input.Name.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"Request name is Already Exist");
            }
        }

        private void ValidCreate(CreateSalaryRequestDto input)
        {
            var isExist = WorkScope.GetAll<SalaryChangeRequest>()
                .Any(x => x.Name.Trim() == input.Name.Trim());
            if (isExist)
            {
                throw new UserFriendlyException($"Request name is Already Exist");
            }
        }

        public async Task<UpdateRequestEmployeeInfoDto> UpdateRequestEmployeeInfo(UpdateRequestEmployeeInfoDto input)
        {
            var rqEmployee = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .Where(x => x.Id == input.Id)
                .FirstOrDefault();

            if (rqEmployee == default)
            {
                throw new UserFriendlyException($"Can't find request with id {input.Id}");
            }

            ObjectMapper.Map(input, rqEmployee);


            await WorkScope.UpdateAsync(rqEmployee);

            if (IsAllowUpdateEmployee(input.Id, input.EmployeeId))
            {
                var employee = WorkScope.GetAll<Employee>()
                        .Where(x => x.Id == input.EmployeeId)
                        .FirstOrDefault();
                employee.UserType = input.ToUserType;
                employee.JobPositionId = input.ToJobPositionId;
                employee.LevelId = input.ToLevelId;
                employee.RealSalary = input.ToSalary;
                await WorkScope.UpdateAsync(employee);
            }

            return input;
        }

        public bool IsAllowUpdateEmployee(long screId, long employeeId)
        {
            var rqEmployee = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.ApplyDate)
                .ThenByDescending(x => x.CreationTime)
                .Select(x=> new
                {
                    x.Id,
                    x.SalaryChangeRequest
                })
                .FirstOrDefault();
            if(rqEmployee.Id == screId && (rqEmployee.SalaryChangeRequest == default || rqEmployee.SalaryChangeRequest.Status == SalaryRequestStatus.Executed))
            {
                return true;
            }
            return false;
            
        }
        private async Task<List<GetDataToImportCheckpointFromFileDto>> GetDataFromFileCheckpoint([FromForm] ImportCheckpointDto input)
        {
            var datas = new List<GetDataToImportCheckpointFromFileDto>();
            using (var stream = new MemoryStream())
            {
                await input.File.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    var rowCount = worksheet.Dimension.End.Row;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var data = new GetDataToImportCheckpointFromFileDto();
                        data.Email = worksheet.Cells[row, 1].GetCellValue<string>() ?? "";
                        data.LevelName = worksheet.Cells[row, 2].GetCellValue<string>() ?? "";
                        data.Salary = worksheet.Cells[row, 3].GetCellValue<double>();
                        data.HasContract = worksheet.Cells[row, 4].GetCellValue<string>().ToLower().Trim() == "yes" ? true : false;
                        data.Row = row;
                        datas.Add(data);
                    }
                }
            }
            return datas;
        }
        public bool ValidDataToImport(GetDataToImportCheckpointFromFileDto data, List<FailResponeDto> failedList)
        {
            var dictLevel = WorkScope.GetAll<Level>()
                                    .Select(s => new { Key = s.Name.ToLower(), s.Id })
                                    .ToDictionary(s => s.Key, s => s.Id);
           
            if (string.IsNullOrEmpty(data.LevelName))
            {
                failedList.Add(new FailResponeDto { Row = data.Row, Email = data.Email, ReasonFail = "Field Level can not be null" });
                return false;

            }
           
            if (!dictLevel.ContainsKey(data.LevelName.ToLower()))
            {
                failedList.Add(new FailResponeDto { Row = data.Row, Email = data.Email, ReasonFail = " Can not found Level" });
                return false;
            }

            return true;
        }

        public async Task<Object> ImportCheckpoint([FromForm] ImportCheckpointDto input)
        {
             ValidImportCheckpoint(input);

            var successList = new List<SalaryChangeRequestEmployee>();
            var failedList = new List<FailResponeDto>();
            var employeeEmailSuccessList = new List<string>();

            var salaryChangeRequest = WorkScope.GetAll<SalaryChangeRequest>()
                .Where(x => x.Id == input.SalaryChangeRequestId)
                .FirstOrDefault();

            var setEmployeeAlreadyExist = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .Where(x => x.SalaryChangeRequestId == input.SalaryChangeRequestId)
                    .Select(x => x.Employee.Email.ToLower().Trim())
                    .Distinct()
                    .ToHashSet();

            var dicEmployee = await WorkScope.GetAll<Employee>()
                .Where(x => x.Status != EmployeeStatus.Quit)
                .ToDictionaryAsync(k => k.Email, v => new DicEmployeeInfoDto
                {
                    Id = v.Id,
                    LevelId = v.LevelId,
                    UserType = v.UserType,
                    RealSalary = v.RealSalary,
                    JobPositionId = v.JobPositionId,
                    ProbationPercentage = v.ProbationPercentage
                });

            var dictLevel = WorkScope.GetAll<Level>()
                                    .Select(s => new { Key = s.Name.ToLower().Trim(), s.Id })
                                    .ToDictionary(s => s.Key, s => s.Id);


            var datas = await GetDataFromFileCheckpoint(input);

 
            foreach (var data in datas)
            {
                if (string.IsNullOrEmpty(data.Email))
                {
                    failedList.Add(new FailResponeDto { Row = data.Row, Email = data.Email, ReasonFail = "Email null or empty" });
                    continue;
                }
                var email = data.Email.ToLower().Trim();
                if (!dicEmployee.ContainsKey(email))
                {
                    failedList.Add(new FailResponeDto { Row = data.Row, Email = email, ReasonFail = "Email not found" });
                    continue;
                }

                if (setEmployeeAlreadyExist.Contains(email) || employeeEmailSuccessList.Contains(email))
                {
                    failedList.Add(new FailResponeDto { Row = data.Row, Email = email, ReasonFail = "Already imported" });
                    continue;
                }

                if (!ValidDataToImport(data, failedList))
                {
                    continue;
                }
                var employee = dicEmployee[data.Email];
                long newLevelId = dictLevel[data.LevelName.ToLower().Trim()];
                var rqEmployee = await CreateChangeRequestAndContract(employee, salaryChangeRequest, data.Salary, data.HasContract, newLevelId);

                employeeEmailSuccessList.Add(data.Email);
                successList.Add(rqEmployee);
            }
            return new { successList, failedList };
        }

        // TODO: rqEmployee.Employee is null, not auto-mapped when test (CreateChangeRequestAndContract_Test1)
        public async Task<SalaryChangeRequestEmployee> CreateChangeRequestAndContract(DicEmployeeInfoDto employee, SalaryChangeRequest salaryChangeRequest, double salary, bool hasContract, long levelId)
        {
            var rqEmployee = new SalaryChangeRequestEmployee
            {
                SalaryChangeRequestId = salaryChangeRequest.Id,
                EmployeeId = employee.Id,
                Type = SalaryRequestType.Change,
                LevelId = employee.LevelId,
                ToLevelId = levelId,
                FromUserType = employee.UserType,
                ToUserType = employee.UserType,
                Salary = employee.RealSalary,
                ToSalary = salary,
                ApplyDate = salaryChangeRequest.ApplyMonth,
                JobPositionId = employee.JobPositionId,
                ToJobPositionId = employee.JobPositionId,
                HasContract = hasContract,
                Note = salaryChangeRequest.Name,
            };

            var reEmployeeId = await WorkScope.InsertAndGetIdAsync(rqEmployee);

            if (rqEmployee.HasContract)
            {
                var contract = new EmployeeContract
                {
                    UserType = rqEmployee.ToUserType,
                    LevelId = rqEmployee.ToLevelId,
                    StartDate = rqEmployee.ApplyDate,
                    EndDate = CommonUtil.GenerateContractEndDate(rqEmployee.ToUserType, rqEmployee.ApplyDate),
                    RealSalary = rqEmployee.ToSalary,
                    BasicSalary = rqEmployee.ToSalary * employee.ProbationPercentage / 100,
                    ProbationPercentage = employee.ProbationPercentage,
                    Code = CommonUtil.GenerateContractCode(rqEmployee.Employee.Email, rqEmployee.ApplyDate.Month, rqEmployee.ApplyDate.Year, rqEmployee.ToUserType),
                    JobPositionId = employee.JobPositionId,
                    SalaryRequestEmployeeId = reEmployeeId,
                    EmployeeId = rqEmployee.EmployeeId,
                    Note = "",
                };

                await WorkScope.InsertAsync(contract);
            }

            return rqEmployee;
        }

        public void ValidImportCheckpoint([FromForm] ImportCheckpointDto input)
        {
            if (input.File == null || !Path.GetExtension(input.File.FileName).Equals(".xlsx"))
            {
                throw new UserFriendlyException("File upload is invalid");
            }
        }

        public MailPreviewInfoDto GetCheckpointTemplate(long requestId)
        {
            MailPreviewInfoDto template = _emailManager.GetEmailContentById(MailFuncEnum.Checkpoint, requestId);
            return template;
        }

        // TODO: can only test exception, cannot test whether send successful or not
        public void SendMailToOneEmployee(SendMailCheckpointDto input)
        {
            var scre = WorkScope.GetAll<SalaryChangeRequestEmployee>()
               .Where(x => x.Id == input.requestId)
               .FirstOrDefault();
            if (scre == default)
            {
                throw new UserFriendlyException($"Can not found salary change request employee with Id = {input.requestId}");
            }
            _emailManager.Send(input.MailContent);
        }

        public string SendMailToAllEmployee(long id, InputGetEmployeeInSalaryRequestDto input)
        {
            var emailTemplate = _emailManager.GetEmailTemplateDto(MailFuncEnum.Checkpoint);
            if (emailTemplate == default)
            {
                throw new UserFriendlyException($"Not found email template for checkpoint");
            }
            var listSalaryChangeRequestEmployees = GetEmployeesInSalaryRequest(id, input).Result.Items;

            List<ResultTemplateEmail<CheckpointMailTemplateDto>> listMails = listSalaryChangeRequestEmployees.Select(x => new ResultTemplateEmail<CheckpointMailTemplateDto>
            {
                Result = new CheckpointMailTemplateDto
                {
                    EmployeeFullName = x.FullName,
                    SendToEmail = x.Email,
                    CheckpointName = x.SalaryChangeRequestName,
                    ApplyDate = x.ApplyMonth.ToString("dd/MM/yyyy"),
                    NewLevel = x.ToLevelName,
                    NewSalary = CommonUtil.FormatDisplayMoney(x.ToSalary),
                    OldSalary = CommonUtil.FormatDisplayMoney(x.Salary),
                    OldLevel = x.LevelName,
                }
            }).ToList();

            var delaySendMail = 0;
            foreach (var mail in listMails)
            {
                MailPreviewInfoDto mailInput = _emailManager.GenerateEmailContent(mail.Result, emailTemplate);
                _backgroundJobManager.Enqueue<SendMail, MailPreviewInfoDto>(mailInput, BackgroundJobPriority.High, TimeSpan.FromSeconds(delaySendMail));
                delaySendMail += HRMv2Consts.DELAY_SEND_MAIL_SECOND;
            }

            return $"Started sending {listMails.Count} email.";
        }

        private void FillMetaLevel(ExcelPackage excelPackageIn)
        {
            var listLevelCodes = WorkScope.GetAll<Level>()
               .Select(x => x.Name.ToLower().Trim())
               .ToList();
            var sheet = excelPackageIn.Workbook.Worksheets[1];
            var rowIndex = 2;
            foreach (var name in listLevelCodes)
            {
                sheet.Cells[rowIndex, 1].Value = name;
                rowIndex++;
            }
        }

        // TODO: Template file must be in folder bin\Debug\net6.0\wwwroot\template which is ignored when push to git (GetTemplateToImportCheckpoint_Test1)
        public async Task<FileBase64Dto> GetTemplateToImportCheckpoint()
        {
            var templateFilePath = Path.Combine(templateFolder, "ImportCheckpoint.xlsx");

            using (var stream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
            {

                using (var memoryStream = new MemoryStream(File.ReadAllBytes(templateFilePath)))
                {
                    using (var package = new ExcelPackage(memoryStream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        FillMetaLevel(package);

                        string fileBase64 = Convert.ToBase64String(package.GetAsByteArray());

                        return new FileBase64Dto
                        {
                            FileName = "ImportCheckpoint",
                            FileType = MimeTypeNames.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
                            Base64 = fileBase64
                        };
                    }
                }
            }
        }
    }
}
