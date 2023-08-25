using Abp.Domain.Entities;
using Abp.UI;
using HRMv2.Core.Tests;
using HRMv2.Entities;
using HRMv2.Manager.Benefits.Dto;
using HRMv2.Manager.Categories.Benefits;
using HRMv2.Manager.Categories.Benefits.Dto;
using HRMv2.Manager.Salaries.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Extension;
using NccCore.Paging;
using NccCore.Uitls;
using Shouldly;
using Xunit;
using static HRMv2.Constants.Enum.HRMEnum;
using DateTime = System.DateTime;

namespace HRMv2.Application.Tests.APIs.BenefitManagerTest
{
    public class BenefitManager_Tests : HRMv2CoreTestBase
    {
        private readonly BenefitManager _benefit;
        private readonly IWorkScope _workScope;

        public BenefitManager_Tests()
        {
            _benefit = Resolve<BenefitManager>();
            _workScope = Resolve<IWorkScope>();
        }

        [Fact]
        public async Task AddEmployeeToBenefit_Should_Add_Employee_To_Benefit()
        {
            var expectBenefitId = 50;
            var expectNumberOfEmployeeInBenefitBefore = 2;
            var expectNumberOfEmployeeInBenefitAfter = 5;

            var addEmployeeToBenefitDto = new AddEmployeeToBenefitDto
            {
                BenefitId = 50,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                ListEmployeeId = new List<long>() { 880, 881, 882 }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var employeeInBenefitBefore = _workScope.GetAll<BenefitEmployee>()
                .Where(x => x.BenefitId == addEmployeeToBenefitDto.BenefitId)
                .Select(x => x.EmployeeId).ToList();

                Assert.Equal(expectNumberOfEmployeeInBenefitBefore, employeeInBenefitBefore.Count);

                var result = await _benefit.AddEmployeeToBenefit(addEmployeeToBenefitDto);
                Assert.NotNull(result);
                Assert.Equal(expectBenefitId, result.BenefitId);
            });

            WithUnitOfWork(() =>
            {
                var listEmployeeIdInBenefit = _workScope.GetAll<BenefitEmployee>()
                .Where(x => x.BenefitId == addEmployeeToBenefitDto.BenefitId)
                .Select(x => x.EmployeeId).ToList();

                Assert.Equal(expectNumberOfEmployeeInBenefitAfter, listEmployeeIdInBenefit.Count);

                Assert.Contains(880, listEmployeeIdInBenefit);
                Assert.Contains(881, listEmployeeIdInBenefit);
                Assert.Contains(882, listEmployeeIdInBenefit);
            });
        }

        [Fact]
        public async Task CloneBenefit_Should_Clone_Benefit_And_BenefitEmployee()
        {
            var expectBenefitId = 63;
            var expectTotalBenefitCount = 16;
            var expectBenefitEmployeeCount = 2;

            var cloneBenefit = new CloneBenefitDto
            {
                BenefitId = 48,
                Name = "Hỗ trợ test",
                Money = 1000000,
                Type = BenefitType.CheDoRieng,
                IsBelongToAllEmployee = false,
                IsCloneEmployee = true,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _benefit.CloneBenefit(cloneBenefit);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allBenefites = _workScope.GetAll<Benefit>();
                var benefit = await _workScope.GetAsync<Benefit>(expectBenefitId);
                var benefitEmployees = _workScope.GetAll<BenefitEmployee>()
                .Where(x => x.BenefitId == expectBenefitId)
                .Select(x => x.EmployeeId).ToList();

                Assert.Equal(expectTotalBenefitCount, allBenefites.Count());

                benefit.Id.ShouldBe(expectBenefitId);
                benefit.Name.ShouldBe(cloneBenefit.Name);
                benefit.Money.ShouldBe(cloneBenefit.Money);
                benefit.Type.ShouldBe(cloneBenefit.Type);
                benefit.IsActive.ShouldBe(true);
                benefit.IsBelongToAllEmployee.ShouldBe(cloneBenefit.IsBelongToAllEmployee);

                Assert.Equal(benefitEmployees.Count, expectBenefitEmployeeCount);
            });
        }

        [Fact]
        public async Task CloneBenefit_Should_Clone_Benefit_And_Not_Clone_BenefitEmployee()
        {
            var expectBenefitId = 63;
            var expectTotalBenefitCount = 16;
            var expectBenefitEmployeeCount = 0;

            var cloneBenefit = new CloneBenefitDto
            {
                BenefitId = 48,
                Name = "Hỗ trợ test",
                Money = 1000000,
                Type = BenefitType.CheDoRieng,
                IsBelongToAllEmployee = false,
                IsCloneEmployee = false,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _benefit.CloneBenefit(cloneBenefit);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allBenefites = _workScope.GetAll<Benefit>();
                var benefit = await _workScope.GetAsync<Benefit>(expectBenefitId);
                var benefitEmployees = _workScope.GetAll<BenefitEmployee>()
                .Where(x => x.BenefitId == expectBenefitId)
                .Select(x => x.EmployeeId).ToList();

                Assert.Equal(expectTotalBenefitCount, allBenefites.Count());

                benefit.Id.ShouldBe(expectBenefitId);
                benefit.Name.ShouldBe(cloneBenefit.Name);
                benefit.Money.ShouldBe(cloneBenefit.Money);
                benefit.Type.ShouldBe(cloneBenefit.Type);
                benefit.IsActive.ShouldBe(true);
                benefit.IsBelongToAllEmployee.ShouldBe(cloneBenefit.IsBelongToAllEmployee);

                Assert.Equal(benefitEmployees.Count, expectBenefitEmployeeCount);
            });
        }

        [Fact]
        public async Task CloneBenefit_Should_Clone_A_Benefit_Not_Exist_And_Not_Clone_BenefitEmployee()
        {
            var expectBenefitId = 63;
            var expectTotalBenefitCount = 16;
            var expectBenefitEmployeeCount = 0;

            var cloneBenefit = new CloneBenefitDto
            {
                BenefitId = 99999,
                Name = "Hỗ trợ test",
                Money = 1000000,
                Type = BenefitType.CheDoRieng,
                IsBelongToAllEmployee = false,
                IsCloneEmployee = true,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _benefit.CloneBenefit(cloneBenefit);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allBenefites = _workScope.GetAll<Benefit>();
                var benefit = await _workScope.GetAsync<Benefit>(expectBenefitId);
                var benefitEmployees = _workScope.GetAll<BenefitEmployee>()
                .Where(x => x.BenefitId == expectBenefitId)
                .Select(x => x.EmployeeId).ToList();

                Assert.Equal(expectTotalBenefitCount, allBenefites.Count());

                benefit.Id.ShouldBe(expectBenefitId);
                benefit.Name.ShouldBe(cloneBenefit.Name);
                benefit.Money.ShouldBe(cloneBenefit.Money);
                benefit.Type.ShouldBe(cloneBenefit.Type);
                benefit.IsActive.ShouldBe(true);
                benefit.IsBelongToAllEmployee.ShouldBe(cloneBenefit.IsBelongToAllEmployee);

                Assert.Equal(benefitEmployees.Count, expectBenefitEmployeeCount);
            });
        }

        [Fact]
        public async Task QuickAddEmployee_Should_Not_Quick_Add_Employee_With_BenefitEmployee_Exist()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var quickAddEmployeeDto = new QuickAddEmployeeDto
                {
                    BenefitId = 50,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today,
                    EmployeeId = 897
                };

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _benefit.QuickAddEmployee(quickAddEmployeeDto);
                });

                Assert.Equal("Employee is already exist in Benenefit", exception.Message);
            });
        }

        [Fact]
        public async Task QuickAddEmployee_Should_Quick_Add_Employee_Valid()
        {
            var expectQuickAddEmployeeDto = new QuickAddEmployeeDto
            {
                BenefitId = 50,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today,
                EmployeeId = 880
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.QuickAddEmployee(expectQuickAddEmployeeDto);
                Assert.NotNull(result);
                Assert.Equal(expectQuickAddEmployeeDto.BenefitId, result.BenefitId);
                Assert.Equal(expectQuickAddEmployeeDto.EmployeeId, result.EmployeeId);
            });

            WithUnitOfWork(() =>
            {
                var benefitEmployee = _workScope.GetAll<BenefitEmployee>()
                .Where(x => x.BenefitId == expectQuickAddEmployeeDto.BenefitId && x.EmployeeId == expectQuickAddEmployeeDto.EmployeeId);
                benefitEmployee.First().BenefitId.ShouldBe(expectQuickAddEmployeeDto.BenefitId);
                benefitEmployee.First().EmployeeId.ShouldBe(expectQuickAddEmployeeDto.EmployeeId);
                benefitEmployee.First().StartDate.ShouldBe<DateTime>((DateTime)expectQuickAddEmployeeDto.StartDate);
                benefitEmployee.First().EndDate.ShouldBe(expectQuickAddEmployeeDto.EndDate);
            });
        }

        [Fact]
        public async Task GetAll_Should_Get_All_Benefits()
        {
            var expectTotalCount = 15;

            WithUnitOfWork(() =>
            {
                var benefits = _benefit.GetAll();

                Assert.Equal(expectTotalCount, benefits.Count());

                benefits.First().Id.ShouldBe(62);
                benefits.First().Name.ShouldBe("Ăn trưa");
                benefits.First().Money.ShouldBe(800000);
                benefits.First().Type.ShouldBe(BenefitType.CheDoChung);
                benefits.First().IsActive.ShouldBe(true);
                benefits.First().IsBelongToAllEmployee.ShouldBe(true);
                benefits.First().CreatorUser.ShouldBe("admin admin");
                benefits.First().UpdatedUser.ShouldBe("admin admin");
                benefits.First().ApplyDate.ShouldBe(new DateTime(2022,12,12,0,0,0));
                DateTimeUtils.ToStringStandardDateTime(benefits.First().CreationTime).ShouldBe("15/12/2022 11:17");
                DateTimeUtils.ToStringStandardDateTime((DateTime)benefits.First().UpdatedTime).ShouldBe("15/12/2022 11:17");
            });
        }

        [Fact]
        public async Task GetAllPaging_Should_Get_All_Benefit_Paging_Test_1()
        {
            var expectTotalCount = 15;
            var expectItemsCount = 12;

            //Total > max, Skip 3 < total
            var filter = new GridParam
            {
                MaxResultCount = 13,
                SkipCount = 3,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetAllPaging(filter);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.Last().Id.ShouldBe(48);
                result.Items.Last().Name.ShouldBe("Hỗ trợ bằng Tiếng Anh");
                result.Items.Last().Money.ShouldBe(1000000);
                result.Items.Last().Type.ShouldBe(BenefitType.CheDoRieng);
                result.Items.Last().IsActive.ShouldBe(true);
                result.Items.Last().IsBelongToAllEmployee.ShouldBe(false);
                result.Items.Last().CreatorUser.ShouldBe("admin admin");
                result.Items.Last().UpdatedUser.ShouldBe("admin admin");
                result.Items.Last().ApplyDate.ShouldBe(new DateTime(2022,1,1,0,0,0));
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().CreationTime).ShouldBe("15/12/2022 11:09");
                DateTimeUtils.ToStringStandardDateTime((DateTime)result.Items.Last().UpdatedTime).ShouldBe("15/12/2022 11:09");
               
            });
        }

        [Fact]
        public async Task GetAllPaging_Should_Get_All_Benefit_Paging_Test_2()
        {
            var expectTotalCount = 15;
            var expectItemsCount = 0;

            //Total > max, Skip 20 > total
            var filter = new GridParam
            {
                MaxResultCount = 13,
                SkipCount = 20,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetAllPaging(filter);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);             
            });
        }

        [Fact]
        public async Task GetAllPaging_Should_Get_All_Benefit_Paging_Test_3()
        {
            var expectTotalCount = 15;
            var expectItemsCount = 5;

            //Total < max, Skip 10 < total
            var filter = new GridParam
            {
                MaxResultCount = 20,
                SkipCount = 10,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetAllPaging(filter);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.Last().Id.ShouldBe(48);
                result.Items.Last().Name.ShouldBe("Hỗ trợ bằng Tiếng Anh");
                result.Items.Last().Money.ShouldBe(1000000);
                result.Items.Last().Type.ShouldBe(BenefitType.CheDoRieng);
                result.Items.Last().IsActive.ShouldBe(true);
                result.Items.Last().IsBelongToAllEmployee.ShouldBe(false);
                result.Items.Last().CreatorUser.ShouldBe("admin admin");
                result.Items.Last().UpdatedUser.ShouldBe("admin admin");
                result.Items.Last().ApplyDate.ShouldBe(new DateTime(2022, 1, 1, 0, 0, 0));
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().CreationTime).ShouldBe("15/12/2022 11:09");
                DateTimeUtils.ToStringStandardDateTime((DateTime)result.Items.Last().UpdatedTime).ShouldBe("15/12/2022 11:09");
            });
        }

        [Fact]
        public async Task GetAllPaging_Should_Get_All_Benefit_Paging_Test_4()
        {
            var expectTotalCount = 15;
            var expectItemsCount = 0;

            //Total < max, Skip 20 > total
            var filter = new GridParam
            {
                MaxResultCount = 20,
                SkipCount = 20,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetAllPaging(filter);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetListEmployeeIdInBenefit_Should_Get_List_Employee_Id_In_Benefit()
        {
            var benefitId = 50;
            var expectTotalCount = 2;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = _benefit.GetListEmployeeIdInBenefit(benefitId);
                Assert.Equal(expectTotalCount, result.Count());
                result.ShouldContain(897);
                result.ShouldContain(886);
            });
        }

        [Fact]
        public async Task GetAllEmployeeNotInBenefit_Should_Get_All_Employees_Not_In_Benefit()
        {
            var benefitId = 60;
            var expectTotalCount = 14;

            WithUnitOfWork(() =>
            {
                var employees = _benefit.GetAllEmployeeNotInBenefit(benefitId);
                Assert.Equal(expectTotalCount, employees.Count());

                employees.Last().Id.ShouldBe(900);
                employees.Last().FullName.ShouldBe("Nguyễn Thị Quỳnh Hoa");
                employees.Last().Email.ShouldBe("hoa.nguyenthiquynh@ncc.asia");
                employees.Last().BranchId.ShouldBe(94);
                employees.Last().LevelId.ShouldBe(322);
                employees.Last().UserTypeName.ShouldBe("Staff");
                employees.Last().Status.ShouldBe(EmployeeStatus.Working);
                DateTimeUtils.ToStringStandardDateTime(employees.Last().StartWorkingDate).ShouldBe("01/01/0001 00:00");
                employees.Last().BranchInfo.Color.ShouldBe("#f44336");
                employees.Last().BranchInfo.Name.ShouldBe("HN1");
                employees.Last().Teams.First().TeamId.ShouldBe(40);
                employees.Last().Teams.First().TeamName.ShouldBe("RenHong");
                employees.Last().UserTypeInfo.Color.ShouldBe("#28a745");
                employees.Last().UserTypeInfo.Name.ShouldBe("Staff");
                employees.Last().JobPositionInfo.Name.ShouldBe("PM");
                employees.Last().JobPositionInfo.Color.ShouldBe("#a62626");
                employees.Last().LevelInfo.Color.ShouldBe("#3bab17");
                employees.Last().LevelInfo.Name.ShouldBe("Middle");
            });
        }

        [Fact]
        public async Task GetBenefitByEmployeeId_Should_Get_Benefit_By_Employee_Id_Test_1()
        {
            var expectTotalCount = 4;
            var expectItemsCount = 2;

            //Total > max, skip 1 < total
            var filter = new GridParam
            {
                MaxResultCount = 2,
                SkipCount = 1,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetBenefitByEmployeeId(880, filter);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.First().BenefitId.ShouldBe(55);
                result.Items.First().BenefitName.ShouldBe("Hỗ trợ chức danh 2M");
                result.Items.First().Money.ShouldBe(2000000);
                result.Items.First().BenefitType.ShouldBe(BenefitType.CheDoRieng);
                result.Items.First().Status.ShouldBe(true);
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().StartDate).ShouldBe("01/01/2022 00:00");
            });
        }

        [Fact]
        public async Task GetBenefitByEmployeeId_Should_Get_Benefit_By_Employee_Id_Test_2()
        {
            var expectTotalCount = 4;
            var expectItemsCount = 0;

            //Total > max, skip 10 > total
            var filter = new GridParam
            {
                MaxResultCount = 3,
                SkipCount = 10,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetBenefitByEmployeeId(880, filter);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetBenefitByEmployeeId_Should_Get_Benefit_By_Employee_Id_Test_3()
        {
            var expectTotalCount = 4;
            var expectItemsCount = 4;

            //Total < max
            var filter = new GridParam
            {
                MaxResultCount = 10
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetBenefitByEmployeeId(880, filter);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.Last().BenefitId.ShouldBe(51);
                result.Items.Last().BenefitName.ShouldBe("Hỗ trợ phong trào");
                result.Items.Last().Money.ShouldBe(2000000);
                result.Items.Last().BenefitType.ShouldBe(BenefitType.CheDoRieng);
                result.Items.Last().Status.ShouldBe(true);
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().StartDate).ShouldBe("01/01/2022 00:00");
            });
        }

        [Fact]
        public async Task GetListMonthFilter_Should_Get_List_Month_Filter()
        {
            var expectTotalCount = 7;

            await WithUnitOfWorkAsync(async () =>
            {
                var dates = await _benefit.GetListMonthFilter();

                Assert.Equal(expectTotalCount, dates.Count());
                DateTimeUtils.ToStringStandardDateTime(dates.First()).ShouldBe("12/12/2022 00:00");
                DateTimeUtils.ToStringStandardDateTime(dates.Last()).ShouldBe("01/01/2022 00:00");
            });
        }

        [Fact]
        public async Task Get_Should_Get_Benefit()
        {
            var expectBenefit = new Benefit
            {
                Id = 48,
                Name = "Hỗ trợ bằng Tiếng Anh",
                Money = 1000000,
                Type = BenefitType.CheDoRieng,
                IsActive = true,
                IsBelongToAllEmployee = false
            };

            WithUnitOfWork(() =>
            {
                var result = _benefit.Get(expectBenefit.Id);

                result.ShouldNotBeNull();
                result.Id.ShouldBe(expectBenefit.Id);
                result.Name.ShouldBe(expectBenefit.Name);
                result.Money.ShouldBe(expectBenefit.Money);
                result.Type.ShouldBe(expectBenefit.Type);
                result.IsActive.ShouldBe(expectBenefit.IsActive);
                result.IsBelongToAllEmployee.ShouldBe(expectBenefit.IsBelongToAllEmployee);
                DateTimeUtils.ToStringStandardDateTime(result.ApplyDate).ShouldBe("01/01/2022 00:00");
                DateTimeUtils.ToStringStandardDateTime(result.CreationTime).ShouldBe("15/12/2022 11:09");
                DateTimeUtils.ToStringStandardDateTime((DateTime)result.UpdatedTime).ShouldBe("15/12/2022 11:09");

                result.CreatorUser.ShouldBe("admin admin");
                result.UpdatedUser.ShouldBe("admin admin");
            });
        }

        [Fact]
        public async Task Create_Should_Create_A_Valid_Benefit()
        {
            var expectBenefit = new Benefit
            {
                Id = 63,
                Name = "Hỗ trợ test",
                Money = 1000000,
                Type = BenefitType.CheDoRieng,
                IsActive = true,
                IsBelongToAllEmployee = false
            };

            var benefit = new BenefitDto
            {
                Name = "Hỗ trợ test",
                Money = 1000000,
                Type = BenefitType.CheDoRieng,
                IsBelongToAllEmployee = false
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.Create(benefit);

                Assert.NotNull(result);
                Assert.Equal(expectBenefit.Name, result.Name);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allBenefitIds = _workScope.GetAll<Benefit>().Select(x => x.Id);
                var benefit = await _workScope.GetAsync<Benefit>(expectBenefit.Id);

                allBenefitIds.Count().ShouldBe(16);
                allBenefitIds.ShouldContain<long>(expectBenefit.Id);

                benefit.Id.ShouldBe(expectBenefit.Id);
                benefit.Name.ShouldBe(expectBenefit.Name);
                benefit.Name.ShouldBe(expectBenefit.Name);
                benefit.Type.ShouldBe(expectBenefit.Type);
                benefit.IsActive.ShouldBe(expectBenefit.IsActive);
                benefit.IsBelongToAllEmployee.ShouldBe(expectBenefit.IsBelongToAllEmployee);
            });
        }

        [Fact]
        public async Task Create_Should_Not_Create_Benefit_With_Name_Exist()
        {
            var benefit = new BenefitDto
            {
                Name = "Ăn trưa",
                Money = 1000000,
                Type = BenefitType.CheDoRieng,
                IsBelongToAllEmployee = false
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _benefit.Create(benefit);
                });

                Assert.Equal("Name is Already Exist", exception.Message);
            });
        }

        [Fact]
        public async Task Update_Should_Update_A_Valid_Benefit()
        {
            var expectBenefit = new BenefitDto
            {
                Id = 62,
                Name = "Ăn trưa updated",
                Money = 850000,
                Type = BenefitType.CheDoRieng,
                IsActive = false,
                IsBelongToAllEmployee = true,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.Update(expectBenefit);

                Assert.NotNull(result);
                Assert.Equal(expectBenefit.Id, result.Id);
                Assert.Equal(expectBenefit.Name, result.Name);
                Assert.Equal(expectBenefit.Money, result.Money);
                Assert.Equal(expectBenefit.Type, result.Type);
                Assert.Equal(expectBenefit.IsActive, result.IsActive);
                Assert.Equal(expectBenefit.IsBelongToAllEmployee, result.IsBelongToAllEmployee);
            });
        }

        [Fact]
        public async Task Update_Should_Not_Update_Benefit_With_Name_Exist()
        {
            var benefit = new BenefitDto
            {
                Id = 62,
                Name = "Hỗ trợ nhà xa",
                Money = 850000,
                Type = BenefitType.CheDoRieng,
                IsActive = false,
                IsBelongToAllEmployee = true
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _benefit.Update(benefit);
                });

                Assert.Equal($"Name is Already Exist", exception.Message);
            });
        }

        [Fact]
        public async Task Update_Should_Not_Update_Benefit_Not_Exist()
        {
            var benefit = new BenefitDto
            {
                Id = 9999,
                Name = "Hỗ trợ test",
                Money = 850000,
                Type = BenefitType.CheDoRieng,
                IsActive = false,
                IsBelongToAllEmployee = true
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _benefit.Update(benefit);
                });

            });
        }

        [Fact]
        public async Task UpdateBenefitEmployee_Should_Update_A_BenefitEmployee()
        {
            var dateTime = System.DateTime.Now;
            var expectEmployeeId = 880;
            var expectBenefitId = 60;

            var updateBEDto = new UpdateBEDto
            {
                Id = 3140,
                BenefitId = expectBenefitId,
                EmployeeId = expectEmployeeId,
                StartDate = dateTime,
                EndDate = dateTime
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.UpdateBenefitEmployee(updateBEDto);

                Assert.NotNull(result);
                Assert.Equal(expectEmployeeId, result.EmployeeId);
                Assert.Equal(expectBenefitId, result.BenefitId);
            });
            await WithUnitOfWorkAsync(async () =>
            {
                var updatedBenefitEmployee = _workScope.GetAsync<BenefitEmployee>(3140).Result;
                Assert.Equal(expectBenefitId, updatedBenefitEmployee.BenefitId);
                Assert.Equal(expectEmployeeId, updatedBenefitEmployee.EmployeeId);
                Assert.Equal(dateTime, updatedBenefitEmployee.StartDate);
                Assert.Equal(dateTime, updatedBenefitEmployee.EndDate);
            });
        }

        [Fact]
        public async Task Delete_Should_Delete_A_Valid_Benefit()
        {
            var expectId = 63;
            var createdId = 0L;

            var benefit = new BenefitDto
            {
                Name = "Hỗ trợ test",
                Money = 1000000,
                Type = BenefitType.CheDoRieng,
                IsBelongToAllEmployee = false
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var entity = _benefit.ObjectMapper.Map<Benefit>(benefit);
                entity.IsActive = true;
                var createdBenefitId = await _workScope.InsertAndGetIdAsync<Benefit>(entity);
                createdId = createdBenefitId;
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.Delete(createdId);
                Assert.Equal(expectId, result);
            });
        }

        [Fact]
        public async Task Delete_Should_Not_Delete_Benefit_That_Employee_Has_Its_Benefit_Id()
        {
            var benefitId = 52;

            await WithUnitOfWorkAsync(async () =>
            {
                var hasNameBenefitEmployee = await _workScope.GetAll<Benefit>()
                .Where(x => x.Id == benefitId).Select(s => s.Name).FirstOrDefaultAsync();

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _benefit.Delete(benefitId);
                });
                Assert.Equal($"Benefit {hasNameBenefitEmployee} has benefit employee", exception.Message);
            });
        }

        [Fact]
        public async Task Delete_Should_Not_Delete_Benefit_Not_Exist()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _benefit.Delete(99999);
                });
            });
        }

        [Fact]
        public async Task ChangeStatus_Should_Update_Status_Benefit()
        {
            var expectBenefitId = 48;
            var expectIsActive = false;

            await WithUnitOfWorkAsync(async () =>
            {
                var updateBenefitStatusDto = new UpdateBenefitStatusDto
                {
                    Id = expectBenefitId,
                    IsActive = expectIsActive,
                };

                var result = await _benefit.ChangeStatus(updateBenefitStatusDto);

                Assert.NotNull(result);
                Assert.Equal(expectIsActive, result.IsActive);
            });
            await WithUnitOfWorkAsync(async () =>
            {
                var updatedBenefit = await _workScope.GetAsync<Benefit>(expectBenefitId);
                Assert.Equal(expectIsActive, updatedBenefit.IsActive);
            });
        }

        [Fact]
        public async Task RemoveEmployeeFromBenefit_Should_Delete_BenefitEmployee()
        {
            var expectId = 3323;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.RemoveEmployeeFromBenefit(expectId);

                Assert.Equal(expectId, result);
            });
        }

        [Fact]
        public async Task DeleteAllBenefitOfEmployee_Should_Delete_All_Benefit_Of_Employee()
        {
            var employeeAfterDelete = 900;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.DeleteAllBenefitOfEmployee(employeeAfterDelete);
                Assert.Equal(employeeAfterDelete, result);
            });

            WithUnitOfWork(() =>
            {
                var benefitOfEmployeeAfterDelete = _workScope.GetAll<BenefitEmployee>().Where(x => x.EmployeeId == employeeAfterDelete);
                Assert.Equal(0, benefitOfEmployeeAfterDelete.Count());
            });
        }

        [Fact]
        public async Task UpdateAllStartDate_Should_Update_All_Start_Date()
        {
            var benefitId = 55;
            var now = System.DateTime.Now;

            await WithUnitOfWorkAsync(async () =>
            {
                var input = new UpdateEmployeeDateDto
                {
                    BenefitId = benefitId,
                    Date = now
                };
                var result = _benefit.UpdateAllStartDate(input);
                Assert.NotNull(result);
                Assert.Equal(input, result);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allBenefitEmployee = _workScope.GetAll<BenefitEmployee>()
                .Where(x => x.BenefitId == benefitId);

                foreach (var benefitEmployee in allBenefitEmployee)
                {
                    Assert.Equal(now, benefitEmployee.StartDate);
                }
            });
        }

        [Fact]
        public async Task UpdateAllEndDate_Should_Update_All_EndDate()
        {
            var dateTime = System.DateTime.Now;
            var expectBenefitId = 62;

            await WithUnitOfWorkAsync(async () =>
            {
                var updateEmployeeEndDateDto = new UpdateEmployeeEndDateDto
                {
                    BenefitId = 62,
                    Date = dateTime
                };

                var result = _benefit.UpdateAllEndDate(updateEmployeeEndDateDto);

                Assert.NotNull(result);
                Assert.Equal(expectBenefitId, result.BenefitId);

            });

            await WithUnitOfWorkAsync(async () =>
            {
                var updatedBenefitEmployee = _workScope.GetAll<BenefitEmployee>().Where(x => x.BenefitId == expectBenefitId);
                foreach (BenefitEmployee benefitEmployee in updatedBenefitEmployee)
                {
                    Assert.Equal(dateTime, benefitEmployee.EndDate);
                }
            });
        }

        [Fact]
        public async Task GetAllBenefitsByEmployeeId_Should_Get_All_Benefits_By_Employee_Id()
        {
            var employeeId = 900;
            var expectTotalCount = 2;

            WithUnitOfWork(() =>
            {
                var benefits = _benefit.GetAllBenefitsByEmployeeId(employeeId);
                Assert.Equal(expectTotalCount, benefits.Count());
                benefits.First().BenefitId.ShouldBe(55);
                benefits.First().BenefitName.ShouldBe("Hỗ trợ chức danh 2M");
                benefits.First().BenefitType.ShouldBe(BenefitType.CheDoRieng);
                benefits.First().Money.ShouldBe(2000000);
                DateTimeUtils.ToStringStandardDateTime(benefits.First().StartDate).ShouldBe("01/01/2022 00:00");
                benefits.First().Status.ShouldBe(true);
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Test_1()
        {
            var expectTotalCount = 3;
            var expectItemsCount = 1;

            //Total > Max, Skip 2 < total
            await WithUnitOfWorkAsync(async () =>
            {
                var benefitEmployeeInputDto = new GetbenefitEmployeeInputDto
                {
                    GridParam = new GridParam
                    {
                        MaxResultCount = 2,
                        SkipCount = 2
                    }
                };

                var result = await _benefit.GetEmployeeInBenefitPaging(55, benefitEmployeeInputDto);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.First().EmployeeId.ShouldBe(900);
                result.Items.First().FullName.ShouldBe("Nguyễn Thị Quỳnh Hoa");
                result.Items.First().Email.ShouldBe("hoa.nguyenthiquynh@ncc.asia");
                result.Items.First().UserTypeName.ShouldBe("Staff");
                result.Items.First().BenefitId.ShouldBe(55);
                result.Items.First().Status.ShouldBe(EmployeeStatus.Working);
                DateTimeUtils.ToStringStandardDateTime(result.Items.First().StartDate).ShouldBe("01/01/2022 00:00");
                result.Items.First().BranchId.ShouldBe(94);
                result.Items.First().BranchInfo.Color.ShouldBe("#f44336");
                result.Items.First().BranchInfo.Name.ShouldBe("HN1");
                result.Items.First().Teams.First().TeamId.ShouldBe(40);
                result.Items.First().Teams.First().TeamName.ShouldBe("RenHong");
                result.Items.First().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.First().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.First().JobPositionId.ShouldBe(49);
                result.Items.First().JobPositionInfo.Name.ShouldBe("PM");
                result.Items.First().JobPositionInfo.Color.ShouldBe("#a62626");
                result.Items.First().LevelId.ShouldBe(322);
                result.Items.First().LevelInfo.Color.ShouldBe("#3bab17");
                result.Items.First().LevelInfo.Name.ShouldBe("Middle");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Test_2()
        {
            var expectTotalCount = 3;
            var expectItemsCount = 0;

            //Total > Max, Skip 10 > total
            await WithUnitOfWorkAsync(async () =>
            {
                var benefitEmployeeInputDto = new GetbenefitEmployeeInputDto
                {
                    GridParam = new GridParam
                    {
                        MaxResultCount = 2,
                        SkipCount = 10
                    }
                };

                var result = await _benefit.GetEmployeeInBenefitPaging(55, benefitEmployeeInputDto);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Test_3()
        {
            var expectTotalCount = 3;
            var expectItemsCount = 3;

            //Total < Max
            await WithUnitOfWorkAsync(async () =>
            {
                var benefitEmployeeInputDto = new GetbenefitEmployeeInputDto
                {
                    GridParam = new GridParam
                    {
                        MaxResultCount = 10
                    }
                };

                var result = await _benefit.GetEmployeeInBenefitPaging(55, benefitEmployeeInputDto);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.Last().EmployeeId.ShouldBe(900);
                result.Items.Last().FullName.ShouldBe("Nguyễn Thị Quỳnh Hoa");
                result.Items.Last().Email.ShouldBe("hoa.nguyenthiquynh@ncc.asia");
                result.Items.Last().UserTypeName.ShouldBe("Staff");
                result.Items.Last().BenefitId.ShouldBe(55);
                result.Items.Last().Status.ShouldBe(EmployeeStatus.Working);
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().StartDate).ShouldBe("01/01/2022 00:00");
                result.Items.Last().BranchId.ShouldBe(94);
                result.Items.Last().BranchInfo.Color.ShouldBe("#f44336");
                result.Items.Last().BranchInfo.Name.ShouldBe("HN1");
                result.Items.Last().Teams.First().TeamId.ShouldBe(40);
                result.Items.Last().Teams.First().TeamName.ShouldBe("RenHong");
                result.Items.Last().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.Last().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.Last().JobPositionId.ShouldBe(49);
                result.Items.Last().JobPositionInfo.Name.ShouldBe("PM");
                result.Items.Last().JobPositionInfo.Color.ShouldBe("#a62626");
                result.Items.Last().LevelId.ShouldBe(322);
                result.Items.Last().LevelInfo.Color.ShouldBe("#3bab17");
                result.Items.Last().LevelInfo.Name.ShouldBe("Middle");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Test_4()
        {
            var expectTotalCount = 3;
            var expectItemsCount = 0;

            //Total < Max, Skip 10 > total
            await WithUnitOfWorkAsync(async () =>
            {
                var benefitEmployeeInputDto = new GetbenefitEmployeeInputDto
                {
                    GridParam = new GridParam
                    {
                        MaxResultCount = 10,
                        SkipCount = 10
                    }
                };

                var result = await _benefit.GetEmployeeInBenefitPaging(55, benefitEmployeeInputDto);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_All_Filter_Test_1()
        {
            var benefitId = 62;
            var expectTotalCount = 1;
            var expectItemsCount = 1;

            //Total < Max, StatusIds count = 2, BranchIds count = 2, UserTypes count = 2, LevelIds count = 2, JobPositionIds count = 1, TeamIds count = 1
            var input = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 5
                },
                StatusIds = new List<EmployeeStatus>
                {
                    EmployeeStatus.Working,
                    EmployeeStatus.Pausing
                },
                BranchIds = new List<long> { 94, 95 },
                UserTypes = new List<UserType>
                {
                   UserType.Staff,
                   UserType.Internship
                },
                LevelIds = new List<long> { 315, 322 },
                JobPositionIds = new List<long>
                {
                  49
                },
                TeamIds = new List<long> { 40 }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(benefitId, input);
                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);
                result.Items.ShouldContain(item => item.EmployeeId == 900);

                result.Items.First().EmployeeId.ShouldBe(900);
                result.Items.First().FullName.ShouldBe("Nguyễn Thị Quỳnh Hoa");
                result.Items.First().Email.ShouldBe("hoa.nguyenthiquynh@ncc.asia");
                result.Items.First().UserTypeName.ShouldBe("Staff");
                result.Items.First().BenefitId.ShouldBe(62);
                result.Items.First().Status.ShouldBe(EmployeeStatus.Working);
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().StartDate).ShouldBe("01/02/2022 00:00");
                result.Items.First().BranchId.ShouldBe(94);
                result.Items.First().BranchInfo.Color.ShouldBe("#f44336");
                result.Items.First().BranchInfo.Name.ShouldBe("HN1");
                result.Items.First().Teams.First().TeamId.ShouldBe(40);
                result.Items.First().Teams.First().TeamName.ShouldBe("RenHong");
                result.Items.First().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.First().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.First().JobPositionId.ShouldBe(49);
                result.Items.First().JobPositionInfo.Name.ShouldBe("PM");
                result.Items.First().JobPositionInfo.Color.ShouldBe("#a62626");
                result.Items.First().LevelId.ShouldBe(322);
                result.Items.First().LevelInfo.Color.ShouldBe("#3bab17");
                result.Items.First().LevelInfo.Name.ShouldBe("Middle");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_All_Filter_Test_2()
        {
            var benefitId = 62;
            var expectTotalCount = 1;
            var expectItemsCount = 0;

            //Total < Max, Skip 5 > total, StatusIds count = 2, BranchIds count = 2, UserTypes count = 2, LevelIds count = 2, JobPositionIds count = 1, TeamIds count = 1
            var input = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 5,
                    SkipCount = 5
                },
                StatusIds = new List<EmployeeStatus>
                {
                    EmployeeStatus.Working,
                    EmployeeStatus.Pausing
                },
                BranchIds = new List<long> { 94, 95 },
                UserTypes = new List<UserType>
                {
                   UserType.Staff,
                   UserType.Internship
                },
                LevelIds = new List<long> { 315, 322 },
                JobPositionIds = new List<long>
                {
                  49
                },
                TeamIds = new List<long> { 40 }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(benefitId, input);
                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_All_Filter_Test_3()
        {
            var benefitId = 62;
            var expectTotalCount = 1;
            var expectItemsCount = 0;

            //Total < Max, Skip 5 > total, StatusIds count = 2, BranchIds count = 2, UserTypes count = 2, LevelIds count = 2, JobPositionIds count = 1, TeamIds count = 1
            var input = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 5,
                    SkipCount = 5
                },
                StatusIds = new List<EmployeeStatus>
                {
                    EmployeeStatus.Working,
                    EmployeeStatus.Pausing
                },
                BranchIds = new List<long> { 94, 95 },
                UserTypes = new List<UserType>
                {
                   UserType.Staff,
                   UserType.Internship
                },
                LevelIds = new List<long> { 315, 322 },
                JobPositionIds = new List<long>
                {
                  48
                },
                TeamIds = new List<long> { 41 }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(benefitId, input);
                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_Status_Filter_Case_1()
        {
            var benefitId = 62;
            var expectTotalCount = 3;
            var expectItemsCount = 1;

            //Total < Max, Skips < total, EmployeeStatus count = 1
            var input = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 5,
                    SkipCount = 2
                },
                StatusIds = new List<EmployeeStatus>
                {
                    EmployeeStatus.Pausing
                },
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(benefitId, input);
                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.First().EmployeeId.ShouldBe(893);
                result.Items.First().FullName.ShouldBe("Trần Diễm Tú");
                result.Items.First().Email.ShouldBe("tu.trandiem@ncc.asia");
                result.Items.First().UserTypeName.ShouldBe("Staff");
                result.Items.First().Status.ShouldBe(EmployeeStatus.Pausing);
                result.Items.First().BenefitId.ShouldBe(62);
                DateTimeUtils.ToStringStandardDateTime(result.Items.First().StartDate).ShouldBe("01/10/2022 00:00");
                result.Items.First().BranchId.ShouldBe(95);
                result.Items.First().BranchInfo.Color.ShouldBe("#17a2b8");
                result.Items.First().BranchInfo.Name.ShouldBe("HN2");
                result.Items.First().Teams.First().TeamId.ShouldBe(45);
                result.Items.First().Teams.First().TeamName.ShouldBe("Supporter");
                result.Items.First().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.First().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.First().JobPositionId.ShouldBe(48);
                result.Items.First().JobPositionInfo.Name.ShouldBe("Tester");
                result.Items.First().JobPositionInfo.Color.ShouldBe("#d20f0f");
                result.Items.First().LevelId.ShouldBe(317);
                result.Items.First().LevelInfo.Color.ShouldBe("#1f75cb");
                result.Items.First().LevelInfo.Name.ShouldBe("Fresher+");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_Status_Filter_Case_2()
        {
            var benefitId = 62;
            var expectTotalCount = 22;
            var expectItemsCount = 2;

            //Total > Max, Skip 20 < total, EmployeeStatus count = 2
            var input = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 10,
                    SkipCount = 20
                },
                StatusIds = new List<EmployeeStatus>
                {
                    EmployeeStatus.Working,
                    EmployeeStatus.Pausing
                },
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(benefitId, input);
                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.Last().EmployeeId.ShouldBe(880);
                result.Items.Last().FullName.ShouldBe("Phạm Thiên An");
                result.Items.Last().Email.ShouldBe("an.phamthien@ncc.asia");
                result.Items.Last().UserTypeName.ShouldBe("Staff");
                result.Items.Last().Status.ShouldBe(EmployeeStatus.Working);
                result.Items.Last().BenefitId.ShouldBe(62);
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().StartDate).ShouldBe("01/10/2022 00:00");
                result.Items.Last().BranchId.ShouldBe(94);
                result.Items.Last().BranchInfo.Color.ShouldBe("#f44336");
                result.Items.Last().BranchInfo.Name.ShouldBe("HN1");
                result.Items.Last().Teams.First().TeamId.ShouldBe(41);
                result.Items.Last().Teams.First().TeamName.ShouldBe("PM");
                result.Items.Last().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.Last().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.Last().JobPositionId.ShouldBe(48);
                result.Items.Last().JobPositionInfo.Name.ShouldBe("Tester");
                result.Items.Last().JobPositionInfo.Color.ShouldBe("#d20f0f");
                result.Items.Last().LevelId.ShouldBe(315);
                result.Items.Last().LevelInfo.Color.ShouldBe("#60b8ff");
                result.Items.Last().LevelInfo.Name.ShouldBe("Fresher-");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_UserType_Filter_Case_1()
        {
            var benefitId = 62;
            var expectTotalCount = 1;
            var expectItemsCount = 1;

            //Total < Max, UserTypes count = 1
            var input = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 10
                },
                UserTypes = new List<UserType>
                {
                   UserType.Internship
                },
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(benefitId, input);
                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.First().EmployeeId.ShouldBe(888);
                result.Items.First().FullName.ShouldBe("Bùi Minh Nhật");
                result.Items.First().Email.ShouldBe("nhat.buiminh@ncc.asia");
                result.Items.First().UserTypeName.ShouldBe("Internship");
                result.Items.First().Status.ShouldBe(EmployeeStatus.Working);
                result.Items.First().BenefitId.ShouldBe(62);
                DateTimeUtils.ToStringStandardDateTime(result.Items.First().StartDate).ShouldBe("15/12/2022 00:00");
                result.Items.First().BranchId.ShouldBe(94);
                result.Items.First().BranchInfo.Color.ShouldBe("#f44336");
                result.Items.First().BranchInfo.Name.ShouldBe("HN1");
                result.Items.First().UserType.ShouldBe(UserType.Internship);
                result.Items.First().UserTypeInfo.Color.ShouldBe("#007bff");
                result.Items.First().UserTypeInfo.Name.ShouldBe("TTS");
                result.Items.First().JobPositionId.ShouldBe(47);
                result.Items.First().JobPositionInfo.Name.ShouldBe("Dev");
                result.Items.First().JobPositionInfo.Color.ShouldBe("#ac3535");
                result.Items.First().LevelId.ShouldBe(314);
                result.Items.First().LevelInfo.Color.ShouldBe("#777");
                result.Items.First().LevelInfo.Name.ShouldBe("Intern_3");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_UserType_Filter_Case_2()
        {
            var benefitId = 62;
            var expectTotalCount = 14;
            var expectItemsCount = 4;

            //Total > Max, Skip 10 < total, UserTypes count = 2
            var input = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 10,
                    SkipCount = 10
                },
                UserTypes = new List<UserType>
                {
                   UserType.Staff,
                   UserType.Internship
                },
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(benefitId, input);
                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.Last().EmployeeId.ShouldBe(880);
                result.Items.Last().FullName.ShouldBe("Phạm Thiên An");
                result.Items.Last().Email.ShouldBe("an.phamthien@ncc.asia");
                result.Items.Last().UserTypeName.ShouldBe("Staff");
                result.Items.Last().Status.ShouldBe(EmployeeStatus.Working);
                result.Items.Last().BenefitId.ShouldBe(62);
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().StartDate).ShouldBe("01/10/2022 00:00");
                result.Items.Last().BranchId.ShouldBe(94);
                result.Items.Last().BranchInfo.Color.ShouldBe("#f44336");
                result.Items.Last().BranchInfo.Name.ShouldBe("HN1");
                result.Items.Last().Teams.First().TeamId.ShouldBe(41);
                result.Items.Last().Teams.First().TeamName.ShouldBe("PM");
                result.Items.Last().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.Last().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.Last().JobPositionId.ShouldBe(48);
                result.Items.Last().JobPositionInfo.Name.ShouldBe("Tester");
                result.Items.Last().JobPositionInfo.Color.ShouldBe("#d20f0f");
                result.Items.Last().LevelId.ShouldBe(315);
                result.Items.Last().LevelInfo.Color.ShouldBe("#60b8ff");
                result.Items.Last().LevelInfo.Name.ShouldBe("Fresher-");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_Job_Position_Filter_Case_1()
        {
            var benefitId = 62;
            var expectTotalCount = 2;
            var expectItemsCount = 1;

            //Total > Max, JobPositionIds count = 1
            var input = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 1
                },
                JobPositionIds = new List<long>
                {
                  50
                },
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(benefitId, input);
                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.Last().EmployeeId.ShouldBe(897);
                result.Items.Last().FullName.ShouldBe("Phạm Khánh Vy");
                result.Items.Last().Email.ShouldBe("vy.phamkhanh@ncc.asia");
                result.Items.Last().UserTypeName.ShouldBe("Staff");
                result.Items.Last().Status.ShouldBe(EmployeeStatus.Working);
                result.Items.Last().BenefitId.ShouldBe(62);
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().StartDate).ShouldBe("01/04/2022 00:00");
                result.Items.Last().BranchId.ShouldBe(95);
                result.Items.Last().BranchInfo.Color.ShouldBe("#17a2b8");
                result.Items.Last().BranchInfo.Name.ShouldBe("HN2");
                result.Items.Last().Teams.First().TeamId.ShouldBe(40);
                result.Items.Last().Teams.First().TeamName.ShouldBe("RenHong");
                result.Items.Last().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.Last().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.Last().JobPositionId.ShouldBe(50);
                result.Items.Last().JobPositionInfo.Name.ShouldBe("Art");
                result.Items.Last().JobPositionInfo.Color.ShouldBe("#c44a4a");
                result.Items.Last().LevelId.ShouldBe(320);
                result.Items.Last().LevelInfo.Color.ShouldBe("#3B2F2F");
                result.Items.Last().LevelInfo.Name.ShouldBe("Junior+");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_Job_Position_Filter_Case_2()
        {
            var benefitId = 62;
            var expectTotalCount = 3;
            var expectItemsCount = 2;

            //Total > Max, Skip 0 < total, JobPositionIds count = 2
            var input = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 2,
                    SkipCount = 0
                },
                JobPositionIds = new List<long>
                {
                  50,55
                },
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(benefitId, input);
                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.First().EmployeeId.ShouldBe(897);
                result.Items.First().FullName.ShouldBe("Phạm Khánh Vy");
                result.Items.First().Email.ShouldBe("vy.phamkhanh@ncc.asia");
                result.Items.First().UserTypeName.ShouldBe("Staff");
                result.Items.First().Status.ShouldBe(EmployeeStatus.Working);
                result.Items.First().BenefitId.ShouldBe(62);
                DateTimeUtils.ToStringStandardDateTime(result.Items.First().StartDate).ShouldBe("01/04/2022 00:00");
                result.Items.First().BranchId.ShouldBe(95);
                result.Items.First().BranchInfo.Color.ShouldBe("#17a2b8");
                result.Items.First().BranchInfo.Name.ShouldBe("HN2");
                result.Items.First().Teams.First().TeamId.ShouldBe(40);
                result.Items.First().Teams.First().TeamName.ShouldBe("RenHong");
                result.Items.First().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.First().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.First().JobPositionId.ShouldBe(50);
                result.Items.First().JobPositionInfo.Name.ShouldBe("Art");
                result.Items.First().JobPositionInfo.Color.ShouldBe("#c44a4a");
                result.Items.First().LevelId.ShouldBe(320);
                result.Items.First().LevelInfo.Color.ShouldBe("#3B2F2F");
                result.Items.First().LevelInfo.Name.ShouldBe("Junior+");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_Branch_Filter_Case_1()
        {
            var expectTotalCount = 2;
            var expectItemsCount = 1;

            //Total < Max, Skip 1 < total, BranchIds count = 1
            var benefitEmployeeInputDto = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 5,
                    SkipCount = 1
                },
                BranchIds = new List<long> { 94 }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(55, benefitEmployeeInputDto);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.Last().EmployeeId.ShouldBe(900);
                result.Items.Last().FullName.ShouldBe("Nguyễn Thị Quỳnh Hoa");
                result.Items.Last().Email.ShouldBe("hoa.nguyenthiquynh@ncc.asia");
                result.Items.Last().UserTypeName.ShouldBe("Staff");
                result.Items.Last().BenefitId.ShouldBe(55);
                result.Items.Last().Status.ShouldBe(EmployeeStatus.Working);
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().StartDate).ShouldBe("01/01/2022 00:00");
                result.Items.Last().BranchId.ShouldBe(94);
                result.Items.Last().BranchInfo.Color.ShouldBe("#f44336");
                result.Items.Last().BranchInfo.Name.ShouldBe("HN1");
                result.Items.Last().Teams.First().TeamId.ShouldBe(40);
                result.Items.Last().Teams.First().TeamName.ShouldBe("RenHong");
                result.Items.Last().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.Last().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.Last().JobPositionId.ShouldBe(49);
                result.Items.Last().JobPositionInfo.Name.ShouldBe("PM");
                result.Items.Last().JobPositionInfo.Color.ShouldBe("#a62626");
                result.Items.Last().LevelId.ShouldBe(322);
                result.Items.Last().LevelInfo.Color.ShouldBe("#3bab17");
                result.Items.Last().LevelInfo.Name.ShouldBe("Middle");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_Branch_Filter_Case_2()
        {
            var expectTotalCount = 3;
            var expectItemsCount = 2;

            //Total > Max, Skip 0 < total, BranchIds count = 2
            var benefitEmployeeInputDto = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 2,
                    SkipCount = 0
                },
                BranchIds = new List<long> { 94, 95 }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(55, benefitEmployeeInputDto);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.First().EmployeeId.ShouldBe(880);
                result.Items.First().FullName.ShouldBe("Phạm Thiên An");
                result.Items.First().Email.ShouldBe("an.phamthien@ncc.asia");
                result.Items.First().UserTypeName.ShouldBe("Staff");
                result.Items.First().Status.ShouldBe(EmployeeStatus.Working);
                result.Items.First().BenefitId.ShouldBe(55);
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().StartDate).ShouldBe("01/01/2022 00:00");
                result.Items.First().BranchId.ShouldBe(94);
                result.Items.First().BranchInfo.Color.ShouldBe("#f44336");
                result.Items.First().BranchInfo.Name.ShouldBe("HN1");
                result.Items.First().Teams.First().TeamId.ShouldBe(41);
                result.Items.First().Teams.First().TeamName.ShouldBe("PM");
                result.Items.First().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.First().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.First().JobPositionId.ShouldBe(48);
                result.Items.First().JobPositionInfo.Name.ShouldBe("Tester");
                result.Items.First().JobPositionInfo.Color.ShouldBe("#d20f0f");
                result.Items.First().LevelId.ShouldBe(315);
                result.Items.First().LevelInfo.Color.ShouldBe("#60b8ff");
                result.Items.First().LevelInfo.Name.ShouldBe("Fresher-");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_LevelIds_Filter_Case_1()
        {
            var expectTotalCount = 1;
            var expectItemsCount = 1;

            //Total = Max, LevelIds count = 1
            var benefitEmployeeInputDto = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 1
                },
                LevelIds = new List<long> { 322 }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(55, benefitEmployeeInputDto);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.Last().EmployeeId.ShouldBe(900);
                result.Items.Last().FullName.ShouldBe("Nguyễn Thị Quỳnh Hoa");
                result.Items.Last().Email.ShouldBe("hoa.nguyenthiquynh@ncc.asia");
                result.Items.Last().UserTypeName.ShouldBe("Staff");
                result.Items.Last().BenefitId.ShouldBe(55);
                result.Items.Last().Status.ShouldBe(EmployeeStatus.Working);
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().StartDate).ShouldBe("01/01/2022 00:00");
                result.Items.Last().BranchId.ShouldBe(94);
                result.Items.Last().BranchInfo.Color.ShouldBe("#f44336");
                result.Items.Last().BranchInfo.Name.ShouldBe("HN1");
                result.Items.Last().Teams.First().TeamId.ShouldBe(40);
                result.Items.Last().Teams.First().TeamName.ShouldBe("RenHong");
                result.Items.Last().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.Last().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.Last().JobPositionId.ShouldBe(49);
                result.Items.Last().JobPositionInfo.Name.ShouldBe("PM");
                result.Items.Last().JobPositionInfo.Color.ShouldBe("#a62626");
                result.Items.Last().LevelId.ShouldBe(322);
                result.Items.Last().LevelInfo.Color.ShouldBe("#3bab17");
                result.Items.Last().LevelInfo.Name.ShouldBe("Middle");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_LevelIds_Filter_Case_2()
        {
            var expectTotalCount = 2;
            var expectItemsCount = 1;

            //Total > Max, Skip 1 < total, LevelIds count = 2
            var benefitEmployeeInputDto = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 1,
                    SkipCount = 1
                },
                LevelIds = new List<long> { 315, 322 }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(55, benefitEmployeeInputDto);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.Last().EmployeeId.ShouldBe(900);
                result.Items.Last().FullName.ShouldBe("Nguyễn Thị Quỳnh Hoa");
                result.Items.Last().Email.ShouldBe("hoa.nguyenthiquynh@ncc.asia");
                result.Items.Last().UserTypeName.ShouldBe("Staff");
                result.Items.Last().BenefitId.ShouldBe(55);
                result.Items.Last().Status.ShouldBe(EmployeeStatus.Working);
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().StartDate).ShouldBe("01/01/2022 00:00");
                result.Items.Last().BranchId.ShouldBe(94);
                result.Items.Last().BranchInfo.Color.ShouldBe("#f44336");
                result.Items.Last().BranchInfo.Name.ShouldBe("HN1");
                result.Items.Last().Teams.First().TeamId.ShouldBe(40);
                result.Items.Last().Teams.First().TeamName.ShouldBe("RenHong");
                result.Items.Last().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.Last().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.Last().JobPositionId.ShouldBe(49);
                result.Items.Last().JobPositionInfo.Name.ShouldBe("PM");
                result.Items.Last().JobPositionInfo.Color.ShouldBe("#a62626");
                result.Items.Last().LevelId.ShouldBe(322);
                result.Items.Last().LevelInfo.Color.ShouldBe("#3bab17");
                result.Items.Last().LevelInfo.Name.ShouldBe("Middle");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_TeamIds_Filter_Case_1()
        {
            var expectTotalCount = 1;
            var expectItemsCount = 1;

            //Total < Max, TeamIds count = 1
            var benefitEmployeeInputDto = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 3
                },
                IsAndCondition = false,
                TeamIds = new List<long> { 41 }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(55, benefitEmployeeInputDto);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.First().EmployeeId.ShouldBe(880);
                result.Items.First().FullName.ShouldBe("Phạm Thiên An");
                result.Items.First().Email.ShouldBe("an.phamthien@ncc.asia");
                result.Items.First().UserTypeName.ShouldBe("Staff");
                result.Items.First().Status.ShouldBe(EmployeeStatus.Working);
                result.Items.First().BenefitId.ShouldBe(55);
                DateTimeUtils.ToStringStandardDateTime(result.Items.First().StartDate).ShouldBe("01/01/2022 00:00");
                result.Items.First().BranchId.ShouldBe(94);
                result.Items.First().BranchInfo.Color.ShouldBe("#f44336");
                result.Items.First().BranchInfo.Name.ShouldBe("HN1");
                result.Items.First().Teams.First().TeamId.ShouldBe(41);
                result.Items.First().Teams.First().TeamName.ShouldBe("PM");
                result.Items.First().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.First().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.First().JobPositionId.ShouldBe(48);
                result.Items.First().JobPositionInfo.Name.ShouldBe("Tester");
                result.Items.First().JobPositionInfo.Color.ShouldBe("#d20f0f");
                result.Items.First().LevelId.ShouldBe(315);
                result.Items.First().LevelInfo.Color.ShouldBe("#60b8ff");
                result.Items.First().LevelInfo.Name.ShouldBe("Fresher-");
            });
        }

        [Fact]
        public async Task GetEmployeeInBenefitPaging_Should_Get_With_TeamIds_Filter_Case_2()
        {
            var expectTotalCount = 2;
            var expectItemsCount = 1;

            //Total < Max, Skip 1 < total, TeamIds count = 2
            var benefitEmployeeInputDto = new GetbenefitEmployeeInputDto
            {
                GridParam = new GridParam
                {
                    MaxResultCount = 3,
                    SkipCount = 1
                },
                TeamIds = new List<long> { 40, 41 }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _benefit.GetEmployeeInBenefitPaging(55, benefitEmployeeInputDto);

                Assert.Equal(expectTotalCount, result.TotalCount);
                Assert.Equal(expectItemsCount, result.Items.Count);

                result.Items.Last().EmployeeId.ShouldBe(900);
                result.Items.Last().FullName.ShouldBe("Nguyễn Thị Quỳnh Hoa");
                result.Items.Last().Email.ShouldBe("hoa.nguyenthiquynh@ncc.asia");
                result.Items.Last().UserTypeName.ShouldBe("Staff");
                result.Items.Last().BenefitId.ShouldBe(55);
                result.Items.Last().Status.ShouldBe(EmployeeStatus.Working);
                DateTimeUtils.ToStringStandardDateTime(result.Items.Last().StartDate).ShouldBe("01/01/2022 00:00");
                result.Items.Last().BranchId.ShouldBe(94);
                result.Items.Last().BranchInfo.Color.ShouldBe("#f44336");
                result.Items.Last().BranchInfo.Name.ShouldBe("HN1");
                result.Items.Last().Teams.First().TeamId.ShouldBe(40);
                result.Items.Last().Teams.First().TeamName.ShouldBe("RenHong");
                result.Items.Last().UserTypeInfo.Color.ShouldBe("#28a745");
                result.Items.Last().UserTypeInfo.Name.ShouldBe("Staff");
                result.Items.Last().JobPositionId.ShouldBe(49);
                result.Items.Last().JobPositionInfo.Name.ShouldBe("PM");
                result.Items.Last().JobPositionInfo.Color.ShouldBe("#a62626");
                result.Items.Last().LevelId.ShouldBe(322);
                result.Items.Last().LevelInfo.Color.ShouldBe("#3bab17");
                result.Items.Last().LevelInfo.Name.ShouldBe("Middle");
            });
        }
    }
}
