using Abp.Domain.Entities;
using Abp.UI;
using Castle.Core.Internal;
using HRMv2.Core.Tests;
using HRMv2.Entities;
using HRMv2.Manager.Categories;
using HRMv2.NccCore;
using NccCore.Paging;
using Shouldly;
using Xunit;


namespace HRMv2.Application.Tests.APIs.BranchManagerTest
{
    public class BranchManager_Tests : HRMv2CoreTestBase
    {
        private readonly BranchManager _branch;
        private readonly IWorkScope _workScope;

        public BranchManager_Tests()
        {
            _branch = Resolve<BranchManager>();
            _workScope = Resolve<IWorkScope>();
        }

        [Fact]
        public async Task Should_Get_All_Branches()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var expectBranchCount = 9;

                var result = _branch.GetAll();

                result.Count.ShouldBe(expectBranchCount);
                result.ShouldContain(branch => branch.Id == 93);
                result.ShouldContain(branch => branch.Code == "HN1");
            });
        }

        [Fact]
        public async Task Should_Get_All_Branches_Paging()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var expectTotalCount = 9;
                var expectItemsCount = 3;

                var filter = new GridParam
                {
                    MaxResultCount = 3,
                    SkipCount = 2,
                };
                var result = await _branch.GetAllPaging(filter);

                result.TotalCount.ShouldBe(expectTotalCount);
                result.Items.Count.ShouldBe(expectItemsCount);
                result.Items.ShouldContain(branch => branch.Id == 95);
                result.Items.ShouldNotContain(branch => branch.Id == 93);
            });
        }

        [Fact]
        public async Task Should_Get_All_Branches_Paging_By_Search_Text()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var expectTotalCount = 2;
                var expectItemsCount = 2;

                var filter = new GridParam
                {
                    SearchText = "sg"
                };
                var result = await _branch.GetAllPaging(filter);

                result.TotalCount.ShouldBe(expectTotalCount);
                result.Items.Count.ShouldBe(expectItemsCount);
                result.Items.ShouldContain(branch => branch.Id == 98);
                result.Items.ShouldNotContain(branch => branch.Id == 93);
            });
        }

        [Fact]
        public async Task Should_Create_A_Valid_Branch()
        {
            var expectBranch = new Branch
            {
                Id = 102,
                Name = "Tokyo",
                ShortName = "Tokyo",
                Code = "Tokyo",
                Address = "Tokyo",
                Color = "blue",
                CompanyPhone = "0969073769",
                CompanyTaxCode = "CompanyTaxCode Tokyo",
                NameInContract = "NameInContract Tokyo",
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var branch = new BranchDto
                {
                    Name = "Tokyo",
                    ShortName = "Tokyo",
                    Code = "Tokyo",
                    Address = "Tokyo",
                    Color = "blue",
                    CompanyPhone = "0969073769",
                    CompanyTaxCode = "CompanyTaxCode Tokyo",
                    NameInContract = "NameInContract Tokyo",
                    CEOFullName = "CEOFullName Tokyo"
                };
                var result = await _branch.Create(branch);
                var allBranch = _workScope.GetAll<Branch>();

                allBranch.Count().ShouldBe(9);
                result.Id.ShouldBeGreaterThan(allBranch.Last().Id);
                result.Id.ShouldBe(branch.Id);
                result.Name.ShouldBe(branch.Name);
                result.ShortName.ShouldBe(branch.ShortName);
                result.Code.ShouldBe(branch.Code);
                result.Address.ShouldBe(branch.Address);
                result.Color.ShouldBe(branch.Color);
                result.CompanyPhone.ShouldBe(branch.CompanyPhone);
                result.CompanyTaxCode.ShouldBe(branch.CompanyTaxCode);
                result.NameInContract.ShouldBe(branch.NameInContract);
                result.CEOFullName.ShouldBe(branch.CEOFullName);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var expectBranchCount = 10;

                var allBranches = _workScope.GetAll<Branch>().ToList();
                var branch = await _workScope.GetAsync<Branch>(expectBranch.Id);

                allBranches.Count.ShouldBe(expectBranchCount);
                branch.Id.ShouldBe(expectBranch.Id);
                branch.Name.ShouldBe(expectBranch.Name);
                branch.ShortName.ShouldBe(expectBranch.ShortName);
                branch.Code.ShouldBe(expectBranch.Code);
                branch.Address.ShouldBe(expectBranch.Address);
                branch.Color.ShouldBe(expectBranch.Color);
                branch.CompanyPhone.ShouldBe(expectBranch.CompanyPhone);
                branch.CompanyTaxCode.ShouldBe(expectBranch.CompanyTaxCode);
                branch.NameInContract.ShouldBe(expectBranch.NameInContract);
                allBranches.Find(branch => branch.Id == expectBranch.Id).ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task Should_Not_Create_Branch_With_Name_Exist()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _branch.Create(new BranchDto
                    {
                        Name = "HN1",
                        ShortName = "Tokyo",
                        Code = "Tokyo",
                        Address = "Tokyo",
                        Color = "blue",
                        CompanyPhone = "0969073769",
                        CompanyTaxCode = "CompanyTaxCode Tokyo",
                        NameInContract = "NameInContract Tokyo",
                        CEOFullName = "CEOFullName Tokyo"
                    });
                });

                Assert.Equal("Name or Code is Already Exist", exception.Message);
            });
        }

        [Fact]
        public async Task Should_Not_Create_Branch_With_Code_Exist()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _branch.Create(new BranchDto
                    {
                        Name = "HN1",
                        ShortName = "Tokyo",
                        Code = "HN1",
                        Address = "Tokyo",
                        Color = "blue",
                        CompanyPhone = "0969073769",
                        CompanyTaxCode = "CompanyTaxCode Tokyo",
                        NameInContract = "NameInContract Tokyo",
                        CEOFullName = "CEOFullName Tokyo"
                    });
                });

                Assert.Equal("Name or Code is Already Exist", exception.Message);
            });
        }

        [Fact]
        public async Task Should_Update_A_Valid_Branch()
        {
            var expectBranch = new Branch
            {
                Id = 94,
                Name = "Hà Nội 1 update",
                ShortName = "HN1 update",
                Code = "HN1UPDATE",
                Address = "Ha Noi",
                Color = "red",
                CompanyPhone = "0969073799",
                CompanyTaxCode = "CompanyTaxCode HN1 update",
            };
            await WithUnitOfWorkAsync(async () =>
            {
                var branch = new BranchDto
                {
                    Id = 94,
                    Name = "Hà Nội 1 update",
                    ShortName = "HN1 update",
                    Code = "HN1UPDATE",
                    Address = "Ha Noi",
                    Color = "red",
                    CompanyPhone = "0969073799",
                    CompanyTaxCode = "CompanyTaxCode HN1 update",
                    CEOFullName = "CEOFullName HN1 update"
                };

                var result = await _branch.Update(branch);

                result.Id.ShouldBe(branch.Id);
                result.Name.ShouldBe(branch.Name);
                result.ShortName.ShouldBe(branch.ShortName);
                result.Code.ShouldBe(branch.Code);
                result.Address.ShouldBe(branch.Address);
                result.Color.ShouldBe(branch.Color);
                result.CompanyPhone.ShouldBe(branch.CompanyPhone);
                result.CompanyTaxCode.ShouldBe(branch.CompanyTaxCode);
                result.NameInContract.ShouldBe(branch.NameInContract);
                result.CEOFullName.ShouldBe(branch.CEOFullName);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var branch = await _workScope.GetAsync<Branch>(expectBranch.Id);

                branch.Id.ShouldBe(expectBranch.Id);
                branch.Name.ShouldBe(expectBranch.Name);
                branch.ShortName.ShouldBe(expectBranch.ShortName);
                branch.Code.ShouldBe(expectBranch.Code);
                branch.Address.ShouldBe(expectBranch.Address);
                branch.Color.ShouldBe(expectBranch.Color);
                branch.CompanyPhone.ShouldBe(expectBranch.CompanyPhone);
                branch.CompanyTaxCode.ShouldBe(expectBranch.CompanyTaxCode);
                branch.NameInContract.ShouldBe(expectBranch.NameInContract);
            });
        }

        [Fact]
        public async Task Should_Not_Update_A_Branch_With_Name_Exist()
        {
            await WithUnitOfWorkAsync(async () =>
            {

                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _branch.Update(new BranchDto
                    {
                        Id = 94,
                        Name = "HN2",
                        ShortName = "HN1",
                        Code = "HN1",
                        Address = "Ha Noi",
                        Color = "red",
                        CompanyPhone = "0969073799",
                        CompanyTaxCode = "CompanyTaxCode",
                        CEOId = 1
                    });
                });

                Assert.Equal("Name or Code is Already Exist", exception.Message);
            });
        }

        [Fact]
        public async Task Should_Not_Update_A_Branch_With_Code_Exist()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _branch.Update(new BranchDto
                    {
                        Id = 94,
                        Name = "Hà Nội 1",
                        ShortName = "HN1",
                        Code = "HN2",
                        Address = "Ha Noi",
                        Color = "red",
                        CompanyPhone = "0969073799",
                        CompanyTaxCode = "CompanyTaxCode",
                        CEOId = 1
                    });
                });

                Assert.Equal("Name or Code is Already Exist", exception.Message);
            });
        }

        [Fact]
        public async Task Should_Delete_A_Valid_Branch()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var expectId = 101;

                var result = await _branch.Delete(101);
                var allBranch = _workScope.GetAll<Branch>();

                result.ShouldBe(expectId);
                allBranch.Count().ShouldBe(9);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allBranch = _workScope.GetAll<Branch>();

                allBranch.Count().ShouldBe(8);
                allBranch.ToArray().Find(branch => branch.Id == 101).ShouldBeNull();
            });
        }

        [Fact]
        public async Task Should_Not_Delete_A_Branch_Not_Exist()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _branch.Delete(-100);
                });
            });
        }

        [Fact]
        public async Task Should_Not_Delete_A_Branch_That_Employee_Has_Its_BranchId()
        {
            var idTest = 93;
            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _branch.Delete(idTest);
                });

                exception.Message.ShouldBe(String.Format("Branch Id {0} has user", idTest));
            });
        }
    }
}
