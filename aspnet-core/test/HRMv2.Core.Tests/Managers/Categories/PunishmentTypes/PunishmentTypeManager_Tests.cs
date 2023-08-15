using Abp.Domain.Entities;
using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Categories.PunishmentTypes;
using HRMv2.Manager.Categories.PunishmentTypes.Dto;
using HRMv2.NccCore;
using Microsoft.AspNetCore.Http;
using NccCore.Paging;
using Shouldly;
using Xunit;

namespace HRMv2.Core.Tests.Managers.Categories.PunishmentTypes
{
    public class PunishmentTypeManager_Tests : HRMv2CoreTestBase
    {
        private readonly PunishmentTypeManager _punishmentTypeManager;
        private readonly IWorkScope _workScope;

        public PunishmentTypeManager_Tests()
        {
            _punishmentTypeManager = Resolve<PunishmentTypeManager>();
            _workScope = Resolve<IWorkScope>();
        }

        [Fact]
        public async Task GetAllPaging_Test1()
        {
            var gridParam = new GridParam
            {
                SkipCount = 1,
            };
            var expectedTotalCount = 3;
            var expectedItemsCount = 2;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _punishmentTypeManager.GetAllPaging(gridParam);

                result.TotalCount.ShouldBe(expectedTotalCount);
                result.Items.Count.ShouldBe(expectedItemsCount);
                result.Items.ShouldContain(punishmentType => punishmentType.Id == 12);
                result.Items.ShouldContain(punishmentType => punishmentType.Id == 13);
            });
        }

        [Fact]
        public async Task GetAllPaging_Test2()
        {
            var gridParam = new GridParam
            {
                MaxResultCount = 10,
                SearchText = "Unlock",
            };
            var expectedTotalCount = 2;
            var expectedItemsCount = 2;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _punishmentTypeManager.GetAllPaging(gridParam);

                result.TotalCount.ShouldBe(expectedTotalCount);
                result.Items.Count.ShouldBe(expectedItemsCount);
                result.Items.ShouldContain(punishmentType => punishmentType.Id == 12);
                result.Items.ShouldContain(punishmentType => punishmentType.Id == 13);
            });
        }

        [Fact]
        public void GetAll_Test1()
        {
            var expectedTotalCount = 3;
            WithUnitOfWork(() =>
            {
                var result = _punishmentTypeManager.GetAll();

                result.Count.ShouldBe(expectedTotalCount);
                result.ShouldContain(punishmentType => punishmentType.Id == 11);
                result.ShouldContain(punishmentType => punishmentType.Name == "Phạt đi muộn");
                result.ShouldContain(punishmentType => punishmentType.IsActive == true);
                result.ShouldContain(punishmentType => punishmentType.Api == "");
            });
        }

        [Fact]
        public async Task Create_Test1()
        {
            // Standard test case
            var newPunishmentType = new PunishmentTypeDto
            {
                Id = 14,
                Name = "Checkout",
                IsActive = false,
                Api = ""
            };
            var expectedId = 14;
            var allPunishmentTypesBeforeCreate = new List<PunishmentType>();

            await WithUnitOfWorkAsync(async () =>
            {
                allPunishmentTypesBeforeCreate = _workScope.GetAll<PunishmentType>().ToList();

                var result = await _punishmentTypeManager.Create(newPunishmentType);

                result.Name.ShouldBe(newPunishmentType.Name);
                result.IsActive.ShouldBe(newPunishmentType.IsActive);
                result.Api.ShouldBe(newPunishmentType.Api);
                result.Id.ShouldBeGreaterThan(allPunishmentTypesBeforeCreate.Last().Id);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var allPunishmentTypesAfterCreate = _workScope.GetAll<PunishmentType>();
                var punishmentType = await _workScope.GetAsync<PunishmentType>(expectedId);

                allPunishmentTypesAfterCreate.Count().ShouldBeGreaterThan(allPunishmentTypesBeforeCreate.Count);
                punishmentType.ShouldNotBeNull();
                punishmentType.Name.ShouldBe(newPunishmentType.Name);
                punishmentType.IsActive.ShouldBe(newPunishmentType.IsActive);
                punishmentType.Api.ShouldBe(newPunishmentType.Api);
            });
        }

        [Fact]
        public async Task Create_Test2()
        {
            // Existed name
            var newPunishmentType = new PunishmentTypeDto
            {
                Name = "Nhân viên Unlock Timesheet"
            };
            var expectedMessage = "Name is Already Exist";

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _punishmentTypeManager.Create(newPunishmentType);
                });

                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public async Task Update_Test1()
        {
            // Standard test case
            var punishmentType = new PunishmentTypeDto
            {
                Id = 11,
                Name = "Checkout",
                IsActive = false,
                Api = ""
            };
            var updateId = punishmentType.Id;

            await WithUnitOfWorkAsync(async () =>
            {
                var result = await _punishmentTypeManager.Update(punishmentType);

                Assert.NotNull(result);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var updatePunishmentType = await _workScope.GetAsync<PunishmentType>(updateId);

                updatePunishmentType.Name.ShouldBe(punishmentType.Name);
                updatePunishmentType.IsActive.ShouldBe(punishmentType.IsActive);
                updatePunishmentType.Api.ShouldBe(punishmentType.Api);
            });
        }

        [Fact]
        public async Task Update_Test2()
        {
            // Existed name
            var punishmentType = new PunishmentTypeDto
            {
                Id = 11,
                Name = "Nhân viên Unlock Timesheet"
            };
            var expectedMessage = "Name is Already Exist";

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _punishmentTypeManager.Update(punishmentType);
                });
                Assert.Equal(expectedMessage, exception.Message);
            });
        }

        [Fact]
        public async Task Delete_Test1()
        {
            // Standard test case
            var punishmentTypeId = 11;
            var allPunishmentTypesBeforeDelete = new List<PunishmentType>();

            await WithUnitOfWorkAsync(async () =>
            {
                allPunishmentTypesBeforeDelete = _workScope.GetAll<PunishmentType>().ToList();

                var result = await _punishmentTypeManager.Delete(punishmentTypeId);

                Assert.Equal(punishmentTypeId, result);
            });

            WithUnitOfWork(() =>
            {
                var allPunishmentTypesAfterDelete = _workScope.GetAll<PunishmentType>();

                allPunishmentTypesAfterDelete.Count().ShouldBeLessThan(allPunishmentTypesBeforeDelete.Count);
                allPunishmentTypesAfterDelete.ToList().Find(punishmentType => punishmentType.Id == punishmentTypeId).ShouldBeNull();
            });
        }

        [Fact]
        public async Task Delete_Test2()
        {
            // Non-existend PunishmentType
            var punishmentTypeId = 5;

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _punishmentTypeManager.Delete(punishmentTypeId);
                });
            });
        }
    }
}
