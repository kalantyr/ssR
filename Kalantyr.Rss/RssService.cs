using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Rss.Model;
using Kalantyr.Rss.Sources;

namespace Kalantyr.Rss
{
    public class RssService: IRssService
    {
        private readonly IReadOnlyCollection<IFeedSource> _sources;

        public RssService(IHttpClientFactory httpClientFactory)
        {
            _sources = new IFeedSource[]
            {
                new FeedSource1(), 
                new SamoletNews(httpClientFactory),
            };
        }

        public async Task<Feed> GetFeedAsync(string feedId, CancellationToken cancellationToken)
        {
            var source = _sources.FirstOrDefault(s => s.Id.Equals(feedId, StringComparison.InvariantCultureIgnoreCase));
            if (source == null)
                throw new Exception($"Feed \"{ feedId}\" not found");
            return await source.GetFeedAsync(cancellationToken);
        }
    }
}
