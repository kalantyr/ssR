using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Rss.Model;

namespace Kalantyr.Rss
{
    public interface IRssService
    {
        Task<Feed> GetFeedAsync(string feedId, CancellationToken cancellationToken);

        /// <summary>
        /// Возвращает ID всех доступных фидов
        /// </summary>
        IReadOnlyCollection<string> GetFeedIds(string feedId);
    }
}
