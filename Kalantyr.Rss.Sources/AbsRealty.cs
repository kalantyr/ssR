using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Kalantyr.Rss.Model;

namespace Kalantyr.Rss.Sources
{
    public class AbsRealty: IFeedSource
    {
        private static readonly IReadOnlyCollection<string> IgnoreWords = new[]
        {
            "продаж",
            "сбербанк",
            "военн"
        };

        private readonly IHttpClientFactory _httpClientFactory;

        public string Id { get; } = nameof(AbsRealty);

        public string Name { get; } = "Абсолют недвижимость";

        public AbsRealty(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<Feed> GetFeedAsync(CancellationToken cancellationToken)
        {
            var headers = new [] {
                "accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9",
                "accept-language: ru-RU,ru;q=0.9",
                "dnt: 1",
                "referer: https://www.absrealty.ru/projects/nekrasovka\r\nsec-ch-ua: \" Not A;Brand\";v=\"99\", \"Chromium\";v=\"90\", \"Google Chrome\";v=\"90\"",
                "sec-ch-ua-mobile: ?0",
                "sec-fetch-dest: document",
                "sec-fetch-mode: navigate",
                "sec-fetch-site: same-origin",
                "sec-fetch-user: ?1",
                "sec-gpc: 1",
                "upgrade-insecure-requests: 1",
                "user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36"
            };

            try
            {
                var client = _httpClientFactory.CreateClient();

                using var request = new HttpRequestMessage(HttpMethod.Get, "https://www.absrealty.ru/news");
                foreach (var line in headers)
                {
                    var i = line.IndexOf(": ");
                    var key = line.Substring(0, i);
                    var value = line.Substring(i + 1);
                    request.Headers.Add(key, value);
                }

                using var res = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
                var content = await res.Content.ReadAsStringAsync(cancellationToken);
                return Parse(content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private Feed Parse(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException(nameof(s));

            var html = new HtmlDocument();
            html.LoadHtml(s);

            var items = new List<FeedItem>();
            var newsNodes = html.DocumentNode.SelectNodes("//div[@id='__layout']/div/main/div/div/div/div/div/div/a");
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
            if (IgnoreWords.Any(w => i.Title.Contains(w, StringComparison.InvariantCultureIgnoreCase)))
                return false;

            return true;
        }

        private static FeedItem Parse(HtmlNode node)
        {
            var timeNode = node.ChildNodes.FindFirst("time");
            var datePublished = timeNode.GetAttributeValue("pubdate", string.Empty);

            var h5 = node.ChildNodes.FindFirst("h5");
            var title = h5.InnerText.Trim(Environment.NewLine.ToCharArray()).Trim();

            return new FeedItem
            {
                DatePublished = DateTimeOffset.Parse(datePublished),
                Title = title
            };
        }
    }
}
