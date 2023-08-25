using Abp.Linq.Extensions;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Common.Dto;
using HRMv2.Manager.EmployeeContracts.Dto;
using HRMv2.Manager.Notifications.Email;
using HRMv2.Manager.Notifications.Email.Dto;
using HRMv2.Manager.SalaryRequests.Dto;
using HRMv2.NccCore;
using HRMv2.UploadFileServices;
using HRMv2.Utils;
using Microsoft.AspNetCore.Mvc;
using NccCore.Extension;
using NccCore.Paging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static HRMv2.Constants.Enum.HRMEnum;

namespace HRMv2.Manager.EmployeeContracts
{
    public class ContractManager : BaseManager
    {
        private readonly UploadFileService _uploadFileService;
        public readonly EmailManager _emailManager;
        public ContractManager(IWorkScope workScope,UploadFileService uploadFileService, EmailManager emailManager): base(workScope)
        {
            _uploadFileService = uploadFileService;
            _emailManager = emailManager;

        }
        public IQueryable<EmployeeContractDto> QueryAllContract()
        {
            var userTypeList = CommonUtil.USERTYPE_COLOR.ToDictionary(x => x.Id);
            return WorkScope.GetAll<EmployeeContract>()
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new EmployeeContractDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    BasicSalary = x.BasicSalary,
                    EmployeeId = x.EmployeeId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    JobPositionId = x.JobPositionId,
                    JobPositionInfo = new BadgeInfoDto
                    {
                        Color = x.JobPosition.Color,
                        Name = x.JobPosition.Name
                    },
                    File = x.File,
                    LevelId = x.LevelId,
                    LevelInfo = new BadgeInfoDto
                    {
                        Name = x.Level.Name,
                        Color = x.Level.Color
                    },
                    UserType = x.UserType,
                    RealSalary = x.RealSalary,
                    ProbationPercentage = x.ProbationPercentage,
                    FilePath = x.FilePath,
                    SalaryRequestEmployeeId = x.SalaryRequestEmployeeId,
                    CreatorUserFullName = x.CreatorUser != null ? x.CreatorUser.FullName : "",
                    LastModifierUserFullName = x.LastModifierUser != null ? x.LastModifierUser.FullName : "Ts tool",
                    LastModifierTime = x.LastModificationTime,
                    CreationTime = x.CreationTime,
                    Note = x.Note,
                    Request = x.SalaryChangeRequestEmployee.SalaryChangeRequest != null ? new RequestInfoDto
                    {
                        Id = x.SalaryChangeRequestEmployee.SalaryChangeRequest.Id,
                        Name = x.SalaryChangeRequestEmployee.SalaryChangeRequest.Name,
                        Status = x.SalaryChangeRequestEmployee.SalaryChangeRequest.Status,
                    } : null,
                });
        }

        public async Task<GridResult<EmployeeContractDto>> GetAllPaging(GridParam input)
        {
            var query = QueryAllContract();
            return await query.GetGridResult(query, input);
        }

        public List<EmployeeContractDto> GetAll()
        {
            return QueryAllContract().ToList();
        }

        public EmployeeContract GetLastestContractOfEmployee(long employeeId)
        {
            return WorkScope.GetAll<EmployeeContract>()
                .Where(x => x.EmployeeId == employeeId)
                .OrderByDescending(x => x.CreationTime)
                .FirstOrDefault();
        }

        public EmployeeContractDto GetContractBySalaryRequest(long requestEmployeeId)
        {
            return QueryAllContract()
                .Where(x => x.SalaryRequestEmployeeId == requestEmployeeId)
                .FirstOrDefault();
        }


        public AddOrUpdateEmployeeRequestDto CreateContractBySalaryRequest(AddOrUpdateEmployeeRequestDto input)
        {
            var dto = new CreateContractDto
            {
                EmployeeId = input.EmployeeId,
                BasicSalary = input.BasicSalary,
                RealSalary = input.ToSalary,
                JobPositionId = input.ToJobPositionId,
                LevelId = input.ToLevelId,
                UserType = input.ToUserType,
                ProbationPercentage = input.ProbationPercentage,
                StartDate = input.ApplyDate,
                SalaryRequestEmployeeId = input.Id,
            };
            var entity = ObjectMapper.Map<EmployeeContract>(dto);

            var generateCodeDto = new GenerateContractCodeDto
            {
                EmployeeId = entity.EmployeeId,
                Year = input.ApplyDate.Year,
                Month = input.ApplyDate.Month,
                JobPositionId = entity.JobPositionId,
                UserType = entity.UserType,
            };
            var contractCode = GenerateContractCode(generateCodeDto);

            entity.Code = contractCode;
            entity.EndDate = input.ContractEndDate ?? GenerateContractEndDate(entity);

            WorkScope.InsertAsync(entity);

            return input;
        }

        public async Task Update(AddOrUpdateEmployeeRequestDto input, EmployeeContract currentContract)
        {     
            currentContract.LevelId = input.ToLevelId;
            currentContract.RealSalary = input.ToSalary;
            currentContract.JobPositionId = input.ToJobPositionId;
            currentContract.UserType = input.ToUserType;
            currentContract.EndDate = input.ContractEndDate;
            currentContract.ProbationPercentage = input.ProbationPercentage;
            currentContract.StartDate = input.ApplyDate;
            currentContract.SalaryRequestEmployeeId = input.Id;
            currentContract.Code = input.ContractCode;
            currentContract.BasicSalary = input.BasicSalary;

            await WorkScope.UpdateAsync(currentContract);
        }

        public void UpdateContracEndDate(long employeeId, DateTime? newEndDate)
        {
            var currentContract = WorkScope.GetAll<EmployeeContract>()
                .Where(x => x.EmployeeId == employeeId)
                .Where(x => (x.SalaryChangeRequestEmployee.SalaryChangeRequest != null &&  x.SalaryChangeRequestEmployee.SalaryChangeRequest.Status == SalaryRequestStatus.Executed) || 
               ( x.SalaryChangeRequestEmployee.SalaryChangeRequest == null)
                )
                .OrderByDescending(x => x.StartDate)
                .ThenByDescending(x=> x.CreationTime)
                .FirstOrDefault();

            currentContract.EndDate = newEndDate;
            WorkScope.UpdateAsync(currentContract);
        }

        public DateTime? GenerateContractEndDate(EmployeeContract employeeContract)
        {
            var contractPeriodMonth = (int)CommonUtil.GetUserType(employeeContract.UserType).ContractPeriodMonth;
            if (contractPeriodMonth == 0)
            {
                return null;
            }
            var endDateAfterAddMonths = employeeContract.StartDate.AddMonths(contractPeriodMonth);
            return endDateAfterAddMonths.Day == employeeContract.StartDate.Day ? endDateAfterAddMonths.AddDays(-1) : endDateAfterAddMonths;
        }

        /// <summary>
        /// - staff: NGUYENTAMVY/10/2022/HĐLĐ-NCC
        ///- ctv: NGUYENTAMVY/10/2022/HĐCTV-NCC
        ///- T.việc: NGUYENTAMVY/10/2022/HĐTV-NCC
        ///- intern: NGUYENTAMVY/10/2022/HĐĐT-NCC
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GenerateContractCode(GenerateContractCodeDto input)
        {
            var email = WorkScope.GetAll<Employee>()
                .Where(x => x.Id == input.EmployeeId)
                .Select(x => x.Email.ToUpper())
                .FirstOrDefault();
            
            return CommonUtil.GenerateContractCode(email, input.Month, input.Year, input.UserType);
        }

        public async Task<UpdateContractNoteDto> UpdateNote(UpdateContractNoteDto input)
        {
            var contract = WorkScope.GetAll<EmployeeContract>()
                .Where(x => x.Id == input.ContractId)
                .FirstOrDefault();
            contract.Note = input.Note;
            await WorkScope.UpdateAsync(contract);
            return input;
        }


        public async Task<string> UploadContractFile([FromForm] ContractFileDto input)
        {
            if (input.File == null)
            {
                throw new UserFriendlyException("No file upload");
            }
            EmployeeContract employeeContract = await WorkScope.GetAsync<EmployeeContract>(input.ContractId);
            if (employeeContract == default)
            {
                throw new UserFriendlyException($"ContractId {input.ContractId} is NOT exist");
            }
            var subFolder = "contractfile";
            String filePath = await _uploadFileService.UploadFile(input.File, subFolder);
            employeeContract.File = filePath;
            await WorkScope.UpdateAsync(employeeContract);
            return filePath;

        }

        public async Task<long> DeleteContractFile(long id)
        {

            EmployeeContract employeeContract = await WorkScope.GetAsync<EmployeeContract>(id);

            if (employeeContract == default)
            {
                throw new UserFriendlyException($"ContractId {id} is NOT exist");
            }
            if (employeeContract != null)
            {
                employeeContract.File = null;
            }
            await WorkScope.UpdateAsync(employeeContract);
            return id;

        }

        public async Task DeleteContract(long id)
        {
            var contract = WorkScope.GetAll<EmployeeContract>()
                .Where(x => x.Id == id)
                .FirstOrDefault();
            if (contract == default)
            {
                throw new UserFriendlyException($"not found contract with id {id}");
            }

            var salaryRequest = WorkScope.GetAll<SalaryChangeRequestEmployee>()
                .Where(x => x.Id == contract.SalaryRequestEmployeeId)
                .FirstOrDefault();

            await WorkScope.DeleteAsync(contract);

            salaryRequest.HasContract = false;
            await WorkScope.UpdateAsync(salaryRequest);
        }

        public async Task<UpdateContractDto> UpdateEmployeeContract(UpdateContractDto input)
        {
            var entity = await WorkScope.GetAsync<EmployeeContract>(input.Id);
            if(entity == default)
            {
                throw new UserFriendlyException($"Can't found employee contract with Id = {entity.Id}");
            }
            ObjectMapper.Map(input, entity);
            await WorkScope.UpdateAsync(entity);
            return input;
        }
        public MailPreviewInfoDto GetContractTemplate(long contractId, MailFuncEnum type)
        {
            MailPreviewInfoDto template = _emailManager.GetContractContentById(type, contractId);
            return template;
        }


    }
}
