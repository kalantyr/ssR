using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Rss.Model;
using Newtonsoft.Json;

namespace Kalantyr.Rss.Sources
{
    public class SamoletNews: IFeedSource
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private static readonly IReadOnlyCollection<string> IgnoreWords = new[]
        {
            "продаж",
            "сбербанк",
            "военн"
        };

        public string Id { get; } = nameof(SamoletNews);

        public string Name { get; } = "Самолет новости";

        public SamoletNews(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<Feed> GetFeedAsync(CancellationToken cancellationToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var s = await client.GetStringAsync("https://samolet.ru/api/news/?project=9", cancellationToken);
                var data = JsonConvert.DeserializeObject<SamoletNewsData>(s);

                return new Feed
                {
                    Id = Id,
                    Title = Name,
                    Items = data.Results
                        .Where(Filter)
                        .Select(GetFeedItem)
                        .ToArray()
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static bool Filter(SamoletNewsRecord r)
        {
            if (IgnoreWords.Any(w => r.Title.Contains(w, StringComparison.InvariantCultureIgnoreCase)))
                return false;

            return true;
        }

        private static FeedItem GetFeedItem(SamoletNewsRecord r)
        {
            return new FeedItem
            {
                Id = r.Id,
                Title = r.Title,
                DatePublished = DateTimeOffset.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.CurrentCulture),
                Url = r.Url,
                ContentText = r.Description
            };
        }
    }

    public class SamoletNewsData
    {
        public SamoletNewsRecord[] Results { get; set; }
    }

    public class SamoletNewsRecord
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Date { get; set; }
        
        public string Url { get; set; }

        public string Description { get; set; }
    }
}
