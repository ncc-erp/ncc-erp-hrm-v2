using HRMv2.Entities;
using HRMv2.NccCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMv2.Manager
{
    public class InitDataTestManager : BaseManager
    {
        public InitDataTestManager(IWorkScope workScope) : base(workScope)
        {
        }

        public async Task<string> InitDataTest(int employeeCount)
        {
            var branches = new List<Branch>()
            {
                new Branch{Name = "HN1", Code = "hn1", Color = "#d73737"},
                new Branch{Name = "HN2", Code = "hn2", Color = "#d73737"},
                new Branch{Name = "Vinh", Code = "vinh", Color = "#d73737"},
                new Branch{Name = "DN", Code = "dn", Color = "#d73737"},
                new Branch{Name = "SG1", Code = "sg1", Color = "#d73737"},
                new Branch{Name = "SG2", Code = "sg2", Color = "#d73737"},
            };

            await WorkScope.InsertRangeAsync(branches);
            CurrentUnitOfWork.SaveChanges();

            var positions = new List<JobPosition>() { 
                new JobPosition{Name = "Dev", Code = "dev", Color = "blue" } ,
                new JobPosition{Name = "Test", Code = "test", Color = "blue" } ,
                new JobPosition{Name = "PM", Code = "pm", Color = "blue" } ,
                new JobPosition{Name = "IT", Code = "it", Color = "blue" } ,
                new JobPosition{Name = "HR", Code = "hr", Color = "blue" } ,
                new JobPosition{Name = "Art", Code = "art", Color = "blue" } ,

            };


            await WorkScope.InsertRangeAsync(positions);
            CurrentUnitOfWork.SaveChanges();

            var levels = new List<Level>() {
                new Level{Name = "Intern0", Code = "intern0", Color = "grey" } ,
                new Level{Name = "Intern1", Code = "intern1", Color = "blue" } ,
                new Level{Name = "Intern2", Code = "intern2", Color = "blue" } ,
                new Level{Name = "Intern3", Code = "intern3", Color = "blue" } ,
                new Level{Name = "Fresher-", Code = "fresher-", Color = "blue" } ,
                new Level{Name = "Fresher", Code = "fresher", Color = "blue" } ,
                new Level{Name = "Fresher+", Code = "fresher+", Color = "blue" } ,
                new Level{Name = "Junior-", Code = "junior-", Color = "blue" } ,
                new Level{Name = "Junior", Code = "junior", Color = "blue" } ,
                new Level{Name = "Junior+", Code = "junior+", Color = "blue" } ,


            };

            await WorkScope.InsertRangeAsync(levels);
            CurrentUnitOfWork.SaveChanges();

            for (int i = 0; i < employeeCount; i++)
            {

            }

            return "";
        }
    }
}
