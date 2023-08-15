using HRMv2.Entities;
using HRMv2.Manager.WorkingHistories.Dtos;
using HRMv2.NccCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HRMv2.Manager.WorkingHistories
{
    public class WorkingHistoryManager : BaseManager
    {
        public WorkingHistoryManager(IWorkScope workScope) : base(workScope)
        {
        }

        /// <summary>
        /// Get List Employee with Working Histories that dateAt <= endDate 
        /// </summary>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<LastEmployeeWorkingHistoryDto> GetLastEmployeeWorkingHistories(DateTime startDate, DateTime endDate)
        {
            var results = WorkScope.GetAll<EmployeeWorkingHistory>()
                .Select(x => new
                {
                    Employee = new
                    {
                        EmployeeId = x.EmployeeId,
                        BranchInfo = new { Name = x.Employee.Branch.Name, Color = x.Employee.Branch.Color },
                        Avatar = x.Employee.Avatar,
                        Email = x.Employee.Email,
                        FullName = x.Employee.FullName,
                        JobPositionInfo = new { Name = x.Employee.JobPosition.Name, Color = x.Employee.JobPosition.Color },
                        LevelInfo = new { Name = x.Employee.Level.Name, Color = x.Employee.Level.Color },
                        UserType = x.Employee.UserType,
                        BranchId = x.Employee.BranchId,
                        Sex = x.Employee.Sex,
                    },
                    DateAt = x.DateAt,
                    x.Status
                })
                .Where(s => s.DateAt.Date <= endDate)
                .ToList()
                .GroupBy(s => s.Employee)
                .Select(s => new LastEmployeeWorkingHistoryDto
                {
                    EmployeeId = s.Key.EmployeeId,
                    BranchId = s.Key.BranchId,
                    Avatar = s.Key.Avatar,
                    Sex = s.Key.Sex,
                    Email = s.Key.Email,
                    FullName = s.Key.FullName,
                    UserType = s.Key.UserType,
                    WorkingHistories = s.OrderByDescending(x => x.DateAt)
                    .Select(x => new StatusDateAtDto { Status = x.Status, DateAt = x.DateAt })
                    .ToList(),

                    WorkingHistoriesInTimeSpan = s.Where(x => x.DateAt >= startDate)
                    .OrderByDescending(x => x.DateAt)
                    .Select(x => new StatusDateAtDto { Status = x.Status, DateAt = x.DateAt })
                    .ToList(),

                    BranchInfo = new() { Name = s.Key.BranchInfo.Name, Color = s.Key.BranchInfo.Color },
                    JobPositionInfo = new() { Name = s.Key.JobPositionInfo.Name, Color = s.Key.JobPositionInfo.Color },
                    LevelInfo = new() { Name = s.Key.LevelInfo.Name, Color = s.Key.LevelInfo.Color }
                }).ToList();
            
            results.ForEach(s =>
            {
                s.DateAt = s.WorkingHistories.Select(x => x.DateAt).FirstOrDefault();
                s.LastStatus = s.WorkingHistories.Select(x => x.Status).FirstOrDefault();
            });

            return results;

        }

    }
}
