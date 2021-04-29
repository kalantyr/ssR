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
    public class SamoletPhoto: IFeedSource
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private static readonly IReadOnlyCollection<string> IgnoreWords = new[]
        {
            "продаж",
            "сбербанк",
            "военн"
        };

        public string Id { get; } = nameof(SamoletPhoto);

        public string Name { get; } = "Самолет фото";

        public SamoletPhoto(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<Feed> GetFeedAsync(CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();
            var s = await client.GetStringAsync("https://samolet.ru/api/dynamic/?project=9", cancellationToken);
            var data = JsonConvert.DeserializeObject<SamoletPhotoData>(s);

            return new Feed
            {
                Id = Id,
                Title = Name,
                Items = data.Results
                    .Select(GetFeedItem)
                    .ToArray()
            };
        }

        private static FeedItem GetFeedItem(SamoletPhotoRecord r)
        {
            return new FeedItem
            {
                Id = r.Id,
                DatePublished = DateTimeOffset.ParseExact(r.Date, "yyyy-MM-dd", CultureInfo.CurrentCulture),
                Url = "https://samolet.ru/project/nekrasovka"
            };
        }
    }

    public class SamoletPhotoData
    {
        public SamoletPhotoRecord[] Results { get; set; }
    }

    public class SamoletPhotoRecord
    {
        public string Id { get; set; }

        public string Date { get; set; }
    }
}
