using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Kalantyr.Rss.Model;

[assembly:InternalsVisibleTo("Kalantyr.Rss.Tests")]

namespace Kalantyr.Rss.Sources
{
    public class Novostroy: IFeedSource
    {
        private const string Address = "https://www.novostroy.ru/buildings/nekrasovka-samolet-development";

        private readonly IHttpClientFactory _httpClientFactory;

        public string Id { get; } = nameof(Novostroy);

        public string Name { get; } = "novostroy_ru";

        public Novostroy(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<Feed> GetFeedAsync(CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, Address);
            using var res = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            var content = await res.Content.ReadAsStringAsync(cancellationToken);
            return Parse(content);
        }

        private Feed Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException(nameof(s));

            var html = new HtmlDocument();
            html.LoadHtml(s);

            var items = new List<FeedItem>();
            var newsNodes = html.DocumentNode.SelectNodes("//div[@id='comments']/div/div[@class='c-s']");
            foreach (var node in newsNodes)
                items.Add(Parse(node));

            return new Feed
            {
                Id = Id,
                Title = Name,
                Items = items.Where(Filter).ToArray()
            };
        }

        private static bool Filter(FeedItem i)
        {
            //if (IgnoreWords.Any(w => i.Title.Contains(w, StringComparison.InvariantCultureIgnoreCase)))
            //    return false;

            return true;
        }

        private static FeedItem Parse(HtmlNode node)
        {
            var timeNode = node.SelectNodes("./div/div/div[@class='c-date']").First();
            var title = node.SelectNodes("./div/p").Single().InnerText;

            return new FeedItem
            {
                Id = (title + timeNode.InnerText).GetHashCode().ToString(),
                DatePublished = ParseDate(timeNode.InnerText),
                Title = title,
                Url = Address
            };
        }

        internal static DateTimeOffset ParseDate(string s)
        {
            var parts = s.Split(" ");

            var year = int.Parse(parts[2]);

            int month;
            switch (parts[1])
            {
                case "января":
                    month = 01;
                    break;
                case "февраля":
                    month = 02;
                    break;
                case "марта":
                    month = 03;
                    break;
                case "апреля":
                    month = 04;
                    break;
                case "мая":
                    month = 05;
                    break;
                case "июня":
                    month = 06;
                    break;
                case "июля":
                    month = 07;
                    break;
                case "августа":
                    month = 08;
                    break;
                case "ментября":
                    month = 09;
                    break;
                default:
                    throw new NotImplementedException();
            }
            var day = int.Parse(parts[0]);

            parts = parts[4].Split(":");
            var hour = int.Parse(parts[0]);
            var minute = int.Parse(parts[1]);

            return new DateTimeOffset(year, month, day, hour, minute, 0, TimeSpan.FromHours(3));
        }
    }
}
