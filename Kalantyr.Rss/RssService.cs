using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Rss.Model;

namespace Kalantyr.Rss
{
    public class RssService: IRssService
    {
        public async Task<IReadOnlyCollection<Feed>> GetFeedsAsync(CancellationToken cancellationToken)
        {
            var feed = new Feed
            {
                Title = "Первый канал",
                Items = new[]
                {
                    new FeedItem
                    {
                        Id = "1",
                        Title = "Новость №1"
                    },
                    new FeedItem
                    {
                        Id = "2",
                        Title = "Новость №2"
                    }
                }
            };
            return new [] { feed };
        }
    }
}
