using Abp.UI;
using HRMv2.Entities;
using HRMv2.Manager.Categories.Skills;
using HRMv2.Manager.Categories.Skills.Dto;
using HRMv2.NccCore;
using Microsoft.EntityFrameworkCore;
using NccCore.Paging;
using Shouldly;
using Xunit;

namespace HRMv2.Core.Tests.Managers.Categories.Skills
{
    public class SkillManager_Tests : HRMv2CoreTestBase
    {
        private readonly SkillManager _skill;
        private readonly IWorkScope _work;

        public SkillManager_Tests()
        {
            _skill = Resolve<SkillManager>();
            _work = Resolve<IWorkScope>();
        }

        [Fact]
        public async Task GetAllPagging_SkipCount()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                GridParam input = new() { SkipCount = 2 };
                var skills = await _skill.GetAllPaging(input);
                Assert.Equal(6, skills.Items.Count);
                skills.Items.ShouldContain(skill => skill.Id == 64);
                skills.Items.ShouldContain(skill => skill.Name == "NodeJs");
                skills.Items.ShouldContain(skill => skill.Code == "NodeJs");
            });
        }

        [Fact]
        public async Task GetAllPagging_SearchText()
        {
            var search = "nod";
            await WithUnitOfWorkAsync(async () =>
            {
                GridParam input = new() { SearchText = search };
                var skills = await _skill.GetAllPaging(input);
                Assert.Equal(1, skills.Items.Count);
                skills.Items.ShouldNotContain(skill => skill.Id == 59);
                skills.Items.ShouldNotContain(skill => skill.Name == "Java");
                skills.Items.ShouldNotContain(skill => skill.Code == "Java");
                skills.Items.ShouldContain(skill => skill.Id == 64);
                skills.Items.ShouldContain(skill => skill.Name == "NodeJs");
                skills.Items.ShouldContain(skill => skill.Code == "NodeJs");
            });
        }

        [Fact]
        public void GetAll()
        {
            WithUnitOfWork(() =>
            {
                var skills = _skill.GetAll();
                Assert.Equal(8, skills.Count);
                skills.ShouldContain(skill => skill.Id == 66);
                skills.ShouldContain(skill => skill.Name == "PHP");
                skills.ShouldContain(skill => skill.Code == "PHP");
            });
        }

        [Fact]
        public async Task Get()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var Id = 64;
                var skills = _skill.Get(Id);
                Assert.Equal(64, skills.Id);
                Assert.Equal("NodeJs", skills.Name);
                Assert.Equal("NodeJs", skills.Code);
            });
        }

        [Fact]
        public async Task Create()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                SkillDto testSkill = new() { Name = "test", Code = "234" };
                var skill = await _skill.Create(testSkill);
                Assert.Equal(67, skill.Id);
                Assert.Equal("test", skill.Name);
                Assert.Equal("234", skill.Code);
            });
        }

        [Fact]
        public async Task Create_SameName_ReturnException()
        {
            WithUnitOfWork(async () =>
            {
                SkillDto testSkill = new() { Name = "NodeJs", Code = "123" };
                var caughtException = await Assert.ThrowsAsync<UserFriendlyException>(async () => await _skill.Create(testSkill));
                Assert.Equal($"Name or Code is Already Exist", caughtException.Message);
            });
        }

        [Fact]
        public async Task Create_SameCode_ReturnException()
        {
            WithUnitOfWork(async () =>
            {
                SkillDto testSkill = new() { Name = "test", Code = "NodeJs" };
                var caughtException = await Assert.ThrowsAsync<UserFriendlyException>(async () => await _skill.Create(testSkill));
                Assert.Equal($"Name or Code is Already Exist", caughtException.Message);
            });
        }

        [Fact]
        public async Task Update()
        {
            WithUnitOfWork(async () =>
            {
                SkillDto testSkill = new() { Id = 60, Name = "new", Code = "123" };
                var skill = await _skill.Update(testSkill);
                Assert.Equal("new", skill.Name);
            });
        }

        [Fact]
        public async Task Update_SameCode_ReturnException()
        {
            WithUnitOfWork(async () =>
            {
                SkillDto testSkill = new() { Id = 60, Name = "new", Code = "Java" };
                var caughtException = await Assert.ThrowsAsync<UserFriendlyException>(async () => await _skill.Update(testSkill));
                Assert.Equal($"Name or Code is Already Exist", caughtException.Message);
            });
        }

        [Fact]
        public async Task Delete()
        {
            var id = 59;
            WithUnitOfWork(async () =>
            {
                var deletedSkillId = await _skill.Delete(id);
                Assert.Equal(id, deletedSkillId);
            });
            WithUnitOfWork(async () =>
            {
                var skills = _work.GetAll<Skill>();
                var skill = await skills.AnyAsync(x => x.Id == id);
                Assert.False(skill);
            });
        }

        [Fact]
        public async Task Delete_ReturnException()
        {
            WithUnitOfWork(async () =>
            {
                var id = 100;
                var caughtException = await Assert.ThrowsAsync<UserFriendlyException>(async () => await _skill.Delete(id));
                Assert.Equal($"There is no Skill with id {id}", caughtException.Message);
            });
        }
    }
}
