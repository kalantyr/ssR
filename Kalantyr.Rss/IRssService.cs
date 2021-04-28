using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Rss.Model;

namespace Kalantyr.Rss
{
    public interface IRssService
    {
        Task<IReadOnlyCollection<Feed>> GetFeedsAsync(CancellationToken cancellationToken);
    }
}
