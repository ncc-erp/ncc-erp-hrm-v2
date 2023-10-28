using HRMv2.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HRMv2.Core.Tests.Utils
{
    public class CommonUtils_Test
    {
        [Theory]
        [InlineData(12000, "12k")]
        [InlineData(20000, "20k")]
        [InlineData(999, "999")]
        [InlineData(1000000, "1M")]
        [InlineData(1200000, "1.2M")]
        [InlineData(0, "0")]
        [InlineData(1000, "1k")]
        public void FormatDisplayMoneyK_ValidInputs_ReturnsFormattedString(double money, string expected)
        {
            string result = CommonUtil.FormatDisplayMoneyK(money);
            Assert.Equal(expected, result);
        }
    }
}
