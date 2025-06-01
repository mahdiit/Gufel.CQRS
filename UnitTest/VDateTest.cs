using Gufel.Date;

namespace Gufel.UnitTest
{
    public class VDateTest
    {
        [Fact]
        public void Constructor_From_Datetime()
        {
            var expectedDateTime = new DateTime(2024, 10, 10);
            var vDate = new VDate(expectedDateTime);

            var dateTime = vDate.ToDateTime();
            Assert.Equal(expectedDateTime, dateTime);
        }

        [Fact]
        public void Cast_From_Datetime_To_PersianDate()
        {
            var dateTime = new DateTime(2025, 06, 01);
            var vDate = new VDate(dateTime);

            var expectedResult = new { Year = 1404, Month = 3, Day = 11 };

            Assert.Equal(vDate.Year, expectedResult.Year);
            Assert.Equal(vDate.Month, expectedResult.Month);
            Assert.Equal(vDate.Day, expectedResult.Day);
        }

        [Fact]
        public void Cast_From_Datetime_To_HijriDate()
        {
            var dateTime = new DateTime(2025, 06, 01);
            var vDate = new VDate(VDateSettingFactory.GetSetting("ar"), dateTime);

            var expectedResult = new { Year = 1446, Month = 12, Day = 05 };

            Assert.Equal(vDate.Year, expectedResult.Year);
            Assert.Equal(vDate.Month, expectedResult.Month);
            Assert.Equal(vDate.Day, expectedResult.Day);
        }

        [Fact]
        public void ToString_PersianDate_Must_Success()
        {
            var dateTime = new DateTime(2025, 06, 01, 14, 10, 23);
            var vDate = new VDate(dateTime);

            var dayString = vDate.ToString("f");
            const string expected = "یکشنبه, 11 خرداد 1404 14:10:23 ب.ظ";

            Assert.Equal(expected, dayString);
        }

        [Fact]
        public void Constructor_From_PersianDate()
        {
            var expectedDate = new DateTime(2025, 06, 01);
            var vDate = new VDate(1404, 3, 11);

            var result = vDate.ToDateTime();
            Assert.Equal(expectedDate, result);
        }

        [Fact]
        public void Operator_Equal()
        {
            var vDate1 = new VDate(1404, 3, 11);
            var vDate2 = new VDate(1404, 3, 11);

            Assert.Equal(vDate1, vDate2);
        }

        [Fact]
        public void Operator_Greater()
        {
            var vDate1 = new VDate(1404, 3, 11);
            var vDate2 = new VDate(1404, 3, 12);

            Assert.True(vDate1 < vDate2);
        }

        [Fact]
        public void Operator_Lower()
        {
            var vDate1 = new VDate(1404, 3, 11);
            var vDate2 = new VDate(1404, 3, 10);

            Assert.True(vDate1 > vDate2);
        }
    }
}
