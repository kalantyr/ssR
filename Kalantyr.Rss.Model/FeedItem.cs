using System;
using Newtonsoft.Json;

namespace Kalantyr.Rss.Model
{
    public class FeedItem
    {
        private DateTimeOffset _datePublished;

        public string Id { get; set; }
        
        public string Title { get; set; }
        
        public string Url { get; set; }

        [JsonProperty(PropertyName = "content_text")]
        public string ContentText { get; set; }

        [JsonProperty(PropertyName = "date_published")]
        public DateTimeOffset DatePublished
        {
            get => _datePublished;
            set
            {
                if (_datePublished == value)
                    return;

                _datePublished = new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, value.Offset);
            }
        }

        public string Icon { get; set; }
    }
}
