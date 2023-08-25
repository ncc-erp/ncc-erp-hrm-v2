using Abp.Domain.Entities;
using Abp.UI;
using Castle.Core.Internal;
using HRMv2.Core.Tests;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Levels;
using HRMv2.Manager.Categories.Levels.Dto;
using HRMv2.NccCore;
using NccCore.Paging;
using Shouldly;
using Xunit;

namespace HRMv2.Application.Tests.APIs.LevelManagerTest
{
    public class LevelManager_Test : HRMv2CoreTestBase
    {
        private readonly LevelManager _level;
        private readonly IWorkScope _workScope;

        public LevelManager_Test()
        {
            _level = Resolve<LevelManager>();
            _workScope = Resolve<IWorkScope>();
        }

        [Fact]
        public async Task GetAll_Should_Get_All_Test()
        {
            // 16 default levels + 16 test data levels
            var expectTotalCount = 16;

            WithUnitOfWork(() =>
            {
                var levels = _level.GetAll();

                Assert.Equal(expectTotalCount, levels.Count);

                levels.First().Id.ShouldBe(316);
                levels.First().Code.ShouldBe("5");
                levels.First().Color.ShouldBe("#318CE7");
                levels.First().Name.ShouldBe("Fresher");
                levels.First().ShortName.ShouldBe("F");

                levels.Last().Id.ShouldBe(324);
                levels.Last().Code.ShouldBe("13");
                levels.Last().Color.ShouldBe("#c36285");
                levels.Last().Name.ShouldBe("Senior-");
                levels.Last().ShortName.ShouldBe("S-");
            });
        }

        [Fact]
        public async Task GetAllPagging_Test1()
        {
            var expectTotalCount = 16;
            var expectItemsCount = 15;

            //Total > Max
            var level = new GridParam
            {
                MaxResultCount = 15,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var levels = await _level.GetAllPaging(level);

                Assert.Equal(expectTotalCount, levels.TotalCount);
                Assert.Equal(expectItemsCount, levels.Items.Count());

                levels.Items.First().Id.ShouldBe(316);
                levels.Items.First().Code.ShouldBe("5");
                levels.Items.First().Color.ShouldBe("#318CE7");
                levels.Items.First().Name.ShouldBe("Fresher");
                levels.Items.First().ShortName.ShouldBe("F");

            });
        }

        [Fact]
        public async Task GetAllPagging_Test2()
        {
            var expectTotalCount = 16;

            // total > max(default = 10), skip < total
            var defaultMaxResultCount = 10;

            var level = new GridParam
            {
                SkipCount = 3,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var levels = await _level.GetAllPaging(level);

                Assert.Equal(expectTotalCount, levels.TotalCount);
                Assert.Equal(defaultMaxResultCount, levels.Items.Count);
                levels.Items.First().Id.ShouldBe(311);
                levels.Items.First().Code.ShouldBe("0");
                levels.Items.First().Color.ShouldBe("#B2BEB5");
                levels.Items.First().Name.ShouldBe("Intern_0");
                levels.Items.First().ShortName.ShouldBe("I0");
            });
        }

        [Fact]
        public async Task GetAllPagging_Test3()
        {
            var expectTotalCount = 16;
            var expectItemsCount = 0;

            // total > max(default = 10), skip > total
            var gridParam = new GridParam
            {
                SkipCount = 35,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var levels = await _level.GetAllPaging(gridParam);

                Assert.Equal(expectTotalCount, levels.TotalCount);
                Assert.Equal(expectItemsCount, levels.Items.Count);
            });
        }

        [Fact]
        public async Task GetAllPagging_Test4()
        {
            var expectTotalCount = 16;
            var expectItemsCount = 16;

            //Total < Max
            var gridParam = new GridParam
            {
                MaxResultCount = 50,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var levels = await _level.GetAllPaging(gridParam);

                Assert.Equal(expectTotalCount, levels.TotalCount);
                Assert.Equal(expectItemsCount, levels.Items.Count());

                levels.Items.First().Id.ShouldBe(316);
                levels.Items.First().Code.ShouldBe("5");
                levels.Items.First().Color.ShouldBe("#318CE7");
                levels.Items.First().Name.ShouldBe("Fresher");
                levels.Items.First().ShortName.ShouldBe("F");
            });
        }

        [Fact]
        public async Task GetAllPagging_Test5()
        {
            var expectTotalCount = 16;
            var expectItemsCount = 12;

            //Total < Max, skip < total
            var gridParam = new GridParam
            {
                MaxResultCount = 50,
                SkipCount = 4,
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var levels = await _level.GetAllPaging(gridParam);

                Assert.Equal(expectTotalCount, levels.TotalCount);
                Assert.Equal(expectItemsCount, levels.Items.Count);

                levels.Items.Last().Id.ShouldBe(324);
                levels.Items.Last().Code.ShouldBe("13");
                levels.Items.Last().Color.ShouldBe("#c36285");
                levels.Items.Last().Name.ShouldBe("Senior-");
                levels.Items.Last().ShortName.ShouldBe("S-");
            });
        }

        [Fact]
        public async Task GetAllPagging_Test6()
        {
            var expectTotalCount = 16;
            var expectItemsCount = 0;

            //Total < Max, skip > total
            var gridParam = new GridParam
            {
                MaxResultCount = 50,
                SkipCount = 40
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var levels = await _level.GetAllPaging(gridParam);

                Assert.Equal(expectTotalCount, levels.TotalCount);
                Assert.Equal(expectItemsCount, levels.Items.Count);
            });
        }

        [Fact]
        public async Task Create_Test1()
        {
            var expectLevelId = 0L;
            var expectTotalCount = 17;

            var expectLevel = new LevelDto
            {
                Name = "Test_Level_1",
                ShortName = "T1",
                Code = "69",
                Color = "#ffffff"
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var newLevel = await _level.Create(expectLevel);

                expectLevelId = newLevel.Id;
                Assert.Equal(expectLevel.Name, newLevel.Name);
                Assert.Equal(expectLevel.ShortName, newLevel.ShortName);
                Assert.Equal(expectLevel.Code, newLevel.Code);
                Assert.Equal(expectLevel.Color, newLevel.Color);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var level = await _workScope.GetAsync<Level>(expectLevelId);
                var allLevels = _workScope.GetAll<Level>();

                Assert.Equal(expectTotalCount, allLevels.Count());

                Assert.Equal(expectLevel.Name, level.Name);
                Assert.Equal(expectLevel.ShortName, level.ShortName);
                Assert.Equal(expectLevel.Code, level.Code);
                Assert.Equal(expectLevel.Color, level.Color);
            });
        }

        [Fact]
        public async Task Create_Test2()
        {
            // Existed Name
            var level = new LevelDto
            {
                Id = 6,
                Name = "Fresher",
                ShortName = "FF",
                Code = "555",
                Color = "#000000",
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _level.Create(level);
                });

                Assert.Equal($"Name or Code is Already Exist", exception.Message);
            });
        }

        [Fact]
        public async Task Create_Test3()
        {
            // Existed Code
            var level = new LevelDto
            {
                Id = 6,
                Name = "Fresher F",
                ShortName = "FF",
                Code = "5",
                Color = "#000000",
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _level.Create(level);
                });

                Assert.Equal($"Name or Code is Already Exist", exception.Message);
            });
        }

        [Fact]
        public async Task Update_Test1()
        {
            // Standard Test Case
            var level = new LevelDto
            {
                Id = 311,
                Name = "Fresher F",
                ShortName = "FF",
                Code = "555",
                Color = "#000000",
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var newLevel = await _level.Update(level);

                Assert.Equal(level.Name, newLevel.Name);
                Assert.Equal(level.ShortName, newLevel.ShortName);
                Assert.Equal(level.Code, newLevel.Code);
                Assert.Equal(level.Color, newLevel.Color);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var updatedLevel = await _workScope.GetAsync<Level>(level.Id);

                Assert.Equal(level.Name, updatedLevel.Name);
                Assert.Equal(level.ShortName, updatedLevel.ShortName);
                Assert.Equal(level.Code, updatedLevel.Code);
                Assert.Equal(level.Color, updatedLevel.Color);
            });
        }

        [Fact]
        public async Task Update_Test2()
        {
            // Existed Name
            var level = new LevelDto
            {
                Id = 6,
                Name = "Fresher",
                ShortName = "FF",
                Code = "555",
                Color = "#000000",
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _level.Update(level);
                });

                Assert.Equal($"Name or Code is Already Exist", exception.Message);
            });
        }

        [Fact]
        public async Task Update_Test3()
        {
            // Existed Code
            var level = new LevelDto
            {
                Id = 6,
                Name = "Fresher F",
                ShortName = "FF",
                Code = "5",
                Color = "#000000",
            };

            await WithUnitOfWorkAsync(async () =>
            {
                var exception = await Assert.ThrowsAsync<UserFriendlyException>(async () =>
                {
                    await _level.Update(level);
                });

                Assert.Equal($"Name or Code is Already Exist", exception.Message);
            });
        }

        [Fact]
        public async Task Delete_Test1()
        {
            long levelId = 325;

            // 16 default + 16 test data - 1
            long expectedLevelCount = 15;

            await WithUnitOfWorkAsync(async () =>
            {
                var actualId = await _level.Delete(levelId);

                Assert.Equal(levelId, actualId);
            });

            WithUnitOfWork(() =>
            {
                var levels = _workScope.GetAll<Level>().ToArray();

                Assert.Equal(expectedLevelCount, levels.Count());

                levels.Find(level => level.Id == levelId).ShouldBeNull();
            });
        }

        [Fact]
        public async Task Delete_Test2()
        {
            long levelId = 99999;

            await WithUnitOfWorkAsync(async () =>
            {
                await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
                {
                    await _level.Delete(levelId);
                });
            });
        }

        [Fact]
        public async Task CreateDefaultLevel_Test()
        {
            var defaultLevelCount = 16;

            WithUnitOfWork(() =>
            {
                _level.CreateDefaultLevel(1);
                var levels = _workScope.GetAll<Level>().ToList();

                // 16 default levels + 16 test data levels
                levels.Count().ShouldBe(defaultLevelCount);

                levels.Any(level => level.Name == "Intern_0" && level.ShortName == "I0" && level.Code == "0" && level.Color == "#B2BEB5").ShouldBeTrue();
                levels.Any(level => level.Name == "Intern_1" && level.ShortName == "I1" && level.Code == "1" && level.Color == "#8F9779").ShouldBeTrue();
                levels.Any(level => level.Name == "Intern_2" && level.ShortName == "I2" && level.Code == "2" && level.Color == "#665D1E").ShouldBeTrue();
                levels.Any(level => level.Name == "Intern_3" && level.ShortName == "I3" && level.Code == "3" && level.Color == "#777").ShouldBeTrue();
                levels.Any(level => level.Name == "Fresher-" && level.ShortName == "F-" && level.Code == "4" && level.Color == "#60b8ff").ShouldBeTrue();
                levels.Any(level => level.Name == "Fresher" && level.ShortName == "F" && level.Code == "5" && level.Color == "#318CE7").ShouldBeTrue();
                levels.Any(level => level.Name == "Fresher+" && level.ShortName == "F+" && level.Code == "6" && level.Color == "#1f75cb").ShouldBeTrue();
                levels.Any(level => level.Name == "Junior-" && level.ShortName == "J-" && level.Code == "7" && level.Color == "#ad9fa1").ShouldBeTrue();
                levels.Any(level => level.Name == "Junior" && level.ShortName == "J" && level.Code == "8" && level.Color == "#A57164").ShouldBeTrue();
                levels.Any(level => level.Name == "Junior+" && level.ShortName == "J+" && level.Code == "9" && level.Color == "#3B2F2F").ShouldBeTrue();
                levels.Any(level => level.Name == "Middle-" && level.ShortName == "M-" && level.Code == "10" && level.Color == "#A4C639").ShouldBeTrue();
                levels.Any(level => level.Name == "Middle" && level.ShortName == "M" && level.Code == "11" && level.Color == "#3bab17").ShouldBeTrue();
                levels.Any(level => level.Name == "Middle+" && level.ShortName == "M+" && level.Code == "12" && level.Color == "#008000").ShouldBeTrue();
                levels.Any(level => level.Name == "Senior-" && level.ShortName == "S-" && level.Code == "13" && level.Color == "#c36285").ShouldBeTrue();
                levels.Any(level => level.Name == "Senior" && level.ShortName == "S" && level.Code == "14" && level.Color == "#AB274F").ShouldBeTrue();
                levels.Any(level => level.Name == "Principal" && level.ShortName == "P" && level.Code == "15" && level.Color == "#902ee1").ShouldBeTrue();
            });
        }
    }
}
