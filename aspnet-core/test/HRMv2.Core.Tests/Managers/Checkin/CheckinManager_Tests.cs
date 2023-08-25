using HRMv2.Manager.CheckIn;
using HRMv2.NccCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Xunit;

namespace HRMv2.Core.Tests.Managers.Checkin
{
    public class CheckinManager_Tests : HRMv2CoreTestBase
    {
        private readonly CheckInManager _checkin;

        public CheckinManager_Tests()
        {
            _checkin = Resolve<CheckInManager>();
        }

        [Fact]
        public async Task GetUserForCheckIn_Should_Get_Working_Users_For_Checkin()
        {
            var expectTotalCount = 19;
            WithUnitOfWork(() =>
            {
                var result = _checkin.GetUserForCheckIn();

                result.Count.ShouldBe(expectTotalCount);

                result.First().Email.ShouldBe("danh.nguyenthanh@ncc.asia");
                result.First().FirstName.ShouldBe("Danh");
                result.First().LastName.ShouldBe("Nguyễn Thanh");

                result[10].Email.ShouldBe("phuc.lehoang@ncc.asia");
                result[10].FirstName.ShouldBe("Phúc");
                result[10].LastName.ShouldBe("Lê Hoàng");

                result.Last().Email.ShouldBe("thinh.luongphu@ncc.asia");
                result.Last().FirstName.ShouldBe("Thịnh");
                result.Last().LastName.ShouldBe("Lương Phú");
            });
        }
    }
}
