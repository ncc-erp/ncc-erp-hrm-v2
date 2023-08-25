using HRMv2.Manager.Salaries.CalculateSalary.Dto;
using HRMv2.Manager.Salaries.Payslips;
using NccCore.Uitls;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HRMv2.Application.Tests
{
    public class PayslipHelper_Tests
    {

        private List<SalaryInputForPayslipDto> listSalary = new List<SalaryInputForPayslipDto>
            {

                new SalaryInputForPayslipDto{
                    Date = new DateTime(2023, 4, 20),
                    EmployeeId = 1,
                    Salary = 9 * 1000000,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.Change,
                    UserType = Constants.Enum.HRMEnum.UserType.Collaborators,
                },
                new SalaryInputForPayslipDto{
                    Date = new DateTime(2023, 1, 17),
                    EmployeeId = 1,
                    Salary = 9 * 1000000,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.Change,
                    UserType = Constants.Enum.HRMEnum.UserType.Staff,
                },
                new SalaryInputForPayslipDto{
                    Date = new DateTime(2022, 12, 17),
                    EmployeeId = 1,
                    Salary = 8 * 1000000,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.BackToWork,
                    UserType = Constants.Enum.HRMEnum.UserType.ProbationaryStaff
                },
                new SalaryInputForPayslipDto{
                    Date = new DateTime(2022, 11, 7),
                    EmployeeId = 1,
                    Salary =  0,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.StopWorking,
                    UserType = Constants.Enum.HRMEnum.UserType.Staff
                },
                new SalaryInputForPayslipDto{
                    Date = new DateTime(2022, 10, 5),
                    EmployeeId = 1,
                    Salary =  10 * 1000000,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.BackToWork,
                    UserType = Constants.Enum.HRMEnum.UserType.Staff
                },
                 new SalaryInputForPayslipDto{
                    Date = new DateTime(2022, 8, 7),
                    EmployeeId = 1,
                    Salary =  0,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.StopWorking,
                    UserType = Constants.Enum.HRMEnum.UserType.Staff
                },
                 new SalaryInputForPayslipDto{
                    Date = new DateTime(2022, 4, 5),
                    EmployeeId = 1,
                    Salary =  10 * 1000000,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.BackToWork,
                    UserType = Constants.Enum.HRMEnum.UserType.Staff
                },
                 new SalaryInputForPayslipDto{
                    Date = new DateTime(2022, 1, 14),
                    EmployeeId = 1,
                    Salary =  3 * 1000000,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.MaternityLeave,
                    UserType = Constants.Enum.HRMEnum.UserType.Staff
                },
                new SalaryInputForPayslipDto{
                    Date = new DateTime(2022, 1, 1),
                    EmployeeId = 1,
                    Salary =  10 * 1000000,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.Change,
                    UserType = Constants.Enum.HRMEnum.UserType.Staff
                },
                new SalaryInputForPayslipDto{
                    Date = new DateTime(2021, 9, 1),
                    EmployeeId = 1,
                    Salary =  6 * 1000000,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.Change,
                    UserType = Constants.Enum.HRMEnum.UserType.Staff
                },
                new SalaryInputForPayslipDto{
                    Date = new DateTime(2021, 7, 1),
                    EmployeeId = 1,
                    Salary =  5 * 1000000,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.Change,
                    UserType = Constants.Enum.HRMEnum.UserType.ProbationaryStaff
                },
                new SalaryInputForPayslipDto{
                    Date = new DateTime(2021, 5, 1),
                    EmployeeId = 1,
                    Salary =  4 * 1000000,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.Change,
                    UserType = Constants.Enum.HRMEnum.UserType.Internship
                }
                ,
                new SalaryInputForPayslipDto{
                    Date = new DateTime(2021, 4, 6),
                    EmployeeId = 1,
                    Salary =  2 * 1000000,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.Initial,
                    UserType = Constants.Enum.HRMEnum.UserType.Internship
                }
            };

        [Fact]
        public void GetListSalaryInputForPayslipDto_Test8()
        {
            List<SalaryInputForPayslipDto> listSalaryInput = new List<SalaryInputForPayslipDto>
            {
                 new SalaryInputForPayslipDto{
                    Date = new DateTime(2022, 10, 11),
                    EmployeeId = 1,
                    Salary =  2 * 1000000,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.MaternityLeave,
                    UserType = Constants.Enum.HRMEnum.UserType.Staff
                },
                new SalaryInputForPayslipDto{
                    Date = new DateTime(2022, 9, 1),
                    EmployeeId = 1,
                    Salary =  4 * 1000000,
                    SalaryType = Constants.Enum.HRMEnum.SalaryRequestType.Initial,
                    UserType = Constants.Enum.HRMEnum.UserType.Staff
                }                
               
            };


            //LUONG THANG 10/2022
            var firstDateOfPayroll = new DateTime(2022, 10, 1);
            var lastDateOfPayroll = firstDateOfPayroll.AddMonths(1).AddDays(-1);

            var ER = listSalaryInput.Skip(listSalaryInput.Count - 2).Take(2).ToList();

            var AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalaryInput, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }
           
        }

        [Fact]
        public void GetListSalaryInputForPayslipDto_Test7()
        {


            //LUONG THANG 11/2022
            var firstDateOfPayroll = new DateTime(2022, 11, 1);
            var lastDateOfPayroll = firstDateOfPayroll.AddMonths(1).AddDays(-1);

            var ER = listSalary.Skip(listSalary.Count - 10).Take(2).ToList();

            var AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

            //LUONG THANG 12/2022
            firstDateOfPayroll = new DateTime(2022, 12, 1);
            lastDateOfPayroll = firstDateOfPayroll.AddMonths(1).AddDays(-1);

            ER = listSalary.Skip(listSalary.Count - 11).Take(1).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

            //LUONG THANG 1/2023
            firstDateOfPayroll = new DateTime(2023, 1, 1);
            lastDateOfPayroll = firstDateOfPayroll.AddMonths(1).AddDays(-1);

            ER = listSalary.Skip(listSalary.Count - 12).Take(2).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

            //LUONG THANG 2/2023
            firstDateOfPayroll = new DateTime(2023, 2, 1);
            lastDateOfPayroll = firstDateOfPayroll.AddMonths(1).AddDays(-1);

            ER = listSalary.Skip(listSalary.Count - 12).Take(1).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

            //LUONG THANG 4/2023
            firstDateOfPayroll = new DateTime(2023, 4, 1);
            lastDateOfPayroll = firstDateOfPayroll.AddMonths(1).AddDays(-1);

            ER = listSalary.Skip(listSalary.Count - 13).Take(2).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

        }

        [Fact]
        public void GetListSalaryInputForPayslipDto_Test6()
        {


            //LUONG THANG 8/2022
            var firstDateOfPayroll = new DateTime(2022, 8, 1);
            var lastDateOfPayroll = new DateTime(2022, 8, 31);

            var ER = listSalary.Skip(listSalary.Count - 8).Take(2).ToList();

            var AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

            //LUONG THANG 9/2022
            firstDateOfPayroll = new DateTime(2022, 9, 1);
            lastDateOfPayroll = firstDateOfPayroll.AddMonths(1).AddDays(-1);

            ER = listSalary.Skip(listSalary.Count - 8).Take(1).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

            //LUONG THANG 10/2022
            firstDateOfPayroll = new DateTime(2022, 10, 1);
            lastDateOfPayroll = firstDateOfPayroll.AddMonths(1).AddDays(-1);

            ER = listSalary.Skip(listSalary.Count - 9).Take(1).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

            //LUONG THANG 11/2022
            firstDateOfPayroll = new DateTime(2022, 11, 1);
            lastDateOfPayroll = firstDateOfPayroll.AddMonths(1).AddDays(-1);

            ER = listSalary.Skip(listSalary.Count - 10).Take(2).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }



        }


        [Fact]
        public void GetListSalaryInputForPayslipDto_Test5()
        {


            //LUONG THANG 4/2022
            var firstDateOfPayroll = new DateTime(2022, 4, 1);
            var lastDateOfPayroll = new DateTime(2022, 4, 30);

            var ER = listSalary.Skip(listSalary.Count - 7).Take(2).ToList();

            var AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

            //LUONG THANG 5/2022
            firstDateOfPayroll = new DateTime(2022, 5, 1);
            lastDateOfPayroll = new DateTime(2022, 5, 31);

            ER = listSalary.Skip(listSalary.Count - 7).Take(2).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }


            //LUONG THANG 6/2022
            firstDateOfPayroll = new DateTime(2022, 6, 1);
            lastDateOfPayroll = new DateTime(2022, 6, 30);

            ER = listSalary.Skip(listSalary.Count - 7).Take(2).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

            //LUONG THANG 7/2022
            firstDateOfPayroll = new DateTime(2022, 7, 1);
            lastDateOfPayroll = new DateTime(2022, 7, 31);

            ER = listSalary.Skip(listSalary.Count - 7).Take(2).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }



        }


        [Fact]
        public void GetListSalaryInputForPayslipDto_Test4()
        {


            //LUONG THANG 1/2022
            var firstDateOfPayroll = new DateTime(2022, 1, 1);
            var lastDateOfPayroll = new DateTime(2022, 1, 31);

            var ER = listSalary.Skip(listSalary.Count - 6).Take(2).ToList();

            var AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

            //LUONG THANG 2/2022
            firstDateOfPayroll = new DateTime(2022, 2, 1);
            lastDateOfPayroll = new DateTime(2022, 3, 1).AddDays(-1);

            ER = listSalary.Skip(listSalary.Count - 6).Take(1).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }


            //LUONG THANG 3/2022
            firstDateOfPayroll = new DateTime(2022, 3, 1);
            lastDateOfPayroll = new DateTime(2022, 3, 31);

            ER = listSalary.Skip(listSalary.Count - 6).Take(1).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }



        }


        [Fact]
        public void GetListSalaryInputForPayslipDto_Test3()
        {


            //LUONG THANG 10/2021
            var firstDateOfPayroll = new DateTime(2021, 10, 1);
            var lastDateOfPayroll = new DateTime(2021, 10, 31);

            var ER = listSalary.Skip(listSalary.Count - 4).Take(1).ToList();

            var AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

            //LUONG THANG 11/2021
            firstDateOfPayroll = new DateTime(2021, 11, 1);
            lastDateOfPayroll = new DateTime(2021, 11, 30);

            ER = listSalary.Skip(listSalary.Count - 4).Take(1).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }


            //LUONG THANG 12/2021
            firstDateOfPayroll = new DateTime(2021, 12, 1);
            lastDateOfPayroll = new DateTime(2021, 12, 31);

            ER = listSalary.Skip(listSalary.Count - 4).Take(1).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }



        }

        [Fact]
        public void GetListSalaryInputForPayslipDto_Test2()
        {


            //LUONG THANG 7/2021
            var firstDateOfPayroll = new DateTime(2021, 7, 1);
            var lastDateOfPayroll = new DateTime(2021, 7, 31);

            var ER = listSalary.Skip(listSalary.Count - 3).Take(1).ToList();

            var AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

            //LUONG THANG 8/2021
            firstDateOfPayroll = new DateTime(2021, 8, 1);
            lastDateOfPayroll = new DateTime(2021, 8, 31);

            ER = listSalary.Skip(listSalary.Count - 3).Take(1).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }


            //LUONG THANG 9/2021
            firstDateOfPayroll = new DateTime(2021, 9, 1);
            lastDateOfPayroll = new DateTime(2021, 9, 30);

            ER = listSalary.Skip(listSalary.Count - 4).Take(2).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }



        }


        [Fact]
        public void GetListSalaryInputForPayslipDto_Test1()
        {


            //LUONG THANG 4/2021
            var firstDateOfPayroll = new DateTime(2021, 4, 1);
            var lastDateOfPayroll = new DateTime(2021, 4, 30);

            var ER = listSalary.Skip(listSalary.Count - 1).Take(1).ToList();

            var AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

            //LUONG THANG 5/2021
            firstDateOfPayroll = new DateTime(2021, 5, 1);
            lastDateOfPayroll = new DateTime(2021, 5, 31);

            ER = listSalary.Skip(listSalary.Count - 2).Take(1).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }


            //LUONG THANG 6/2021
            firstDateOfPayroll = new DateTime(2021, 6, 1);
            lastDateOfPayroll = new DateTime(2021, 6, 30);

            ER = listSalary.Skip(listSalary.Count - 2).Take(1).ToList();

            AR = PayslipHelper.GetListSalaryInputForPayslipDto(listSalary, firstDateOfPayroll, lastDateOfPayroll);

            // Assert
            AR.Count.ShouldBeEquivalentTo(ER.Count);
            for (int i = 0; i < AR.Count; i++)
            {
                AR.Skip(i).Take(1).FirstOrDefault()?.Date.ShouldBeEquivalentTo(ER.Skip(i).Take(1).FirstOrDefault()?.Date);
            }

        }

    }
}
