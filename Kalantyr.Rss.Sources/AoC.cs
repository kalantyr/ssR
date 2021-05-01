using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Rss.Model;
using Newtonsoft.Json;

namespace Kalantyr.Rss.Sources
{
    public class AoC: IFeedSource
    {
        private const string Address = "https://ashesofcreation.com/news";

        private readonly IHttpClientFactory _httpClientFactory;

        public string Id { get; } = nameof(AoC);

        public string Name { get; } = "AoC";

        public AoC(IHttpClientFactory httpClientFactory)
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

        private Feed Parse(string content)
        {
            var i = content.IndexOf("\"latestNews\"");
            if (i < 0)
                throw new Exception("block \"latestNews\" not fond");

            i = content.IndexOf("{", i);
            var count = 1;
            for (var j = i + 1; j < content.Length; j++)
            {
                if (content[j] == '{')
                    count++;
                if (content[j] == '}')
                    count--;
                if (count == 0)
                {
                    var s = content.Substring(i, j - i + 1);
                    var data = JsonConvert.DeserializeObject<D1>(s);
                    var items = data.Items.Select(it =>
                    {
                        var contentText = it.Fields.BlogContent.Replace(@"\n\n", Environment.NewLine);
                        for (var k = 0; k < contentText.Length; k++)
                        {
                            if (char.IsLetterOrDigit(contentText[k]) || char.IsWhiteSpace(contentText[k]) || char.IsPunctuation(contentText[k]))
                                continue;
                            contentText = contentText.Substring(0, k) + "...";
                            break;
                        }

                        return new FeedItem
                        {
                            Id = it.Fields.Id,
                            Title = it.Fields.Title,
                            DatePublished = it.Sys.CreatedAt,
                            ContentText = contentText,
                            Url = Address + "/" + it.Fields.Slug,
                            Icon = CreateAddress(it.Fields?.FeaturedImage?.Fields?.File?.Url)
                        };
                    }).ToArray();
                    return new Feed
                    {
                        Id = Id,
                        Title = Name,
                        Items = items
                    };
                }
            }

            throw new NotImplementedException();
        }

        private static string CreateAddress(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            return Uri.UriSchemeHttps + Uri.SchemeDelimiter + url.Replace("_~s~", "/").Trim('/');
        }

        public class D1
        {
            public D2[] Items { get; set; }
        }

        public class D2
        {
            public Fields Fields { get; set; }

            public Sys Sys { get; set; }
        }

        public class Fields
        {
            public string Id { get; set; }

            public string Title { get; set; }

            public string Slug { get; set; }

            public string BlogContent { get; set; }

            public FeaturedImage FeaturedImage { get; set; }
        }

        public class Sys
        {
            public DateTimeOffset CreatedAt { get; set; }
        }

        public class FeaturedImage
        {
            public FeaturedImageFields Fields { get; set; }
        }

        public class FeaturedImageFields
        {
            public FeaturedImageFieldsFile File { get; set; }
        }

        public class FeaturedImageFieldsFile
        {
            public string Url { get; set; }
        }
    }
}
