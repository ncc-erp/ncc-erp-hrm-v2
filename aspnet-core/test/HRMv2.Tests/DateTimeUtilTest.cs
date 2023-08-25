using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace HRMv2.Tests
{
    public class DateTimeUtilTest : HRMv2TestBase
    {

        [Fact]
        public void  GetListMonthForDebtPlan_Test()
        {
            // Act
            var output = DateTimeUtils.GetListMonthForDebtPaymentPlan(new DateTime(2022, 01, 01), new DateTime(2022, 05, 01));

            // Assert
            output.Count.ShouldBeEquivalentTo(4);
            output.FirstOrDefault().Year.ShouldBeEquivalentTo(2022);
            output.FirstOrDefault().Month.ShouldBeEquivalentTo(1);

            output.LastOrDefault().Year.ShouldBeEquivalentTo(2022);
            output.LastOrDefault().Month.ShouldBeEquivalentTo(4);


            // Act
            var output2 = DateTimeUtils.GetListMonthForDebtPaymentPlan(new DateTime(2022, 01, 14), new DateTime(2022, 05, 31));

            // Assert
            output2.Count.ShouldBeEquivalentTo(5);
            output2.FirstOrDefault().Year.ShouldBeEquivalentTo(2022);
            output2.FirstOrDefault().Month.ShouldBeEquivalentTo(1);

            output2.LastOrDefault().Year.ShouldBeEquivalentTo(2022);
            output2.LastOrDefault().Month.ShouldBeEquivalentTo(5);

            // Act
            var output3 = DateTimeUtils.GetListMonthForDebtPaymentPlan(new DateTime(2022, 01, 14), new DateTime(2022, 05, 02));

            // Assert
            output3.Count.ShouldBeEquivalentTo(5);
            output3.FirstOrDefault().Year.ShouldBeEquivalentTo(2022);
            output3.FirstOrDefault().Month.ShouldBeEquivalentTo(1);

            output3.LastOrDefault().Year.ShouldBeEquivalentTo(2022);
            output3.LastOrDefault().Month.ShouldBeEquivalentTo(5);
        }
    }
}
