namespace Kalantyr.Rss.Model
{
    public class Feed
    {
        public string Version { get; } = "https://jsonfeed.org/version/1.1";

        public string Title { get; set; }

        public FeedItem[] Items { get; set; } = new FeedItem[0];
        
        public string Id { get; set; }
    }
}
