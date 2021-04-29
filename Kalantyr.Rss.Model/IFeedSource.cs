using System.Threading;
using System.Threading.Tasks;

namespace Kalantyr.Rss.Model
{
    public interface IFeedSource
    {
        string Id { get; }

        string Name { get; }

        Task<Feed> GetFeedAsync(CancellationToken cancellationToken);
    }
}
