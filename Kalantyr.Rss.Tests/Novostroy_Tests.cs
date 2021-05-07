using Kalantyr.Rss.Sources;
using NUnit.Framework;

namespace Kalantyr.Rss.Tests
{
    public class Novostroy_Tests
    {
        [TestCase("6 мая 2021 в 09:31", 2021, 05, 06, 09, 31)]
        [TestCase("30 апреля 2021 в 08:19", 2021, 04, 30, 08, 19)]
        public void ParseDate_Test(string s, int year, int month, int day, int hour, int minute)
        {
            var result = Novostroy.ParseDate(s);
            Assert.AreEqual(year, result.Year);
            Assert.AreEqual(month, result.Month);
            Assert.AreEqual(day, result.Day);
            Assert.AreEqual(hour, result.Hour);
            Assert.AreEqual(minute, result.Minute);
        }
    }
}
