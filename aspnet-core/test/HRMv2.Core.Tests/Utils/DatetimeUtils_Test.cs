using NccCore.Uitls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HRMv2.Core.Tests.Utils
{
    public class DatetimeUtils_Test
    {

        [Theory]
        [InlineData()]
        public void GetListDate_ValidInputs_ReturnListDate()
        {
            var startDate = new DateTime(2023, 3, 2);
            var endDate = new DateTime(2023, 7, 2);
            var expected =
            new List<DateTime> {
                new DateTime(2023, 3, 1),
                new DateTime(2023, 4, 1),
                new DateTime(2023, 5, 1),
                new DateTime(2023, 6, 1),
                new DateTime(2023, 7, 1)
            };


            List<DateTime> result = DateTimeUtils.GetListDate(startDate, endDate);


            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData()]
        public void GetListDate_ValidInputs1_ReturnListDate()
        {
            var startDate = new DateTime(2023, 5, 2,12,1,53);
            var endDate = new DateTime(2024, 3, 2,23,2,43);
            var expected =
            new List<DateTime> {
                new DateTime(2023, 5, 1),
                new DateTime(2023, 6, 1),
                new DateTime(2023, 7, 1),
                new DateTime(2023, 8, 1),
                new DateTime(2023, 9, 1),
                new DateTime(2023, 10, 1),
                new DateTime(2023, 11, 1),
                new DateTime(2023, 12, 1),
                new DateTime(2024, 1, 1),
                new DateTime(2024, 2, 1),
                new DateTime(2024, 3, 1)
            };


            List<DateTime> result = DateTimeUtils.GetListDate(startDate, endDate);


            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData()]
        public void GetListDate_InValidInputs_ReturnListDate()
        {
            var startDate = new DateTime(2023, 5, 2, 12, 1, 53);
            var endDate = new DateTime(2024, 3, 2, 23, 2, 43);
            var expected =
            new List<DateTime> {
                new DateTime(2023, 5, 1),
                new DateTime(2023, 6, 1),
                new DateTime(2023, 7, 1),
                new DateTime(2023, 8, 1),
                new DateTime(2023, 9, 1),
                new DateTime(2023, 10, 1),
                new DateTime(2023, 11, 1),
                new DateTime(2023, 12, 1),
                new DateTime(2024, 1, 1),
                new DateTime(2024, 2, 1),
                new DateTime(2024, 3, 1)
            };


            List<DateTime> result = DateTimeUtils.GetListDate(startDate, endDate);


            Assert.Equal(expected, result);
        }
    }
}
