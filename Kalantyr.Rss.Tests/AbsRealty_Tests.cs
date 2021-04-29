using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Rss.Sources;
using Moq;
using NUnit.Framework;

namespace Kalantyr.Rss.Tests
{
    public class AbsRealty_Tests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory = new Mock<IHttpClientFactory>();

        public AbsRealty_Tests()
        {
            _httpClientFactory
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient());
        }

        [Test]
        [Explicit]
        public async Task GetFeedAsync_Test()
        {
            var absRealty = new AbsRealty(_httpClientFactory.Object);
            var feed = await absRealty.GetFeedAsync(CancellationToken.None);
            Assert.IsNotNull(feed);
        }
    }
}
