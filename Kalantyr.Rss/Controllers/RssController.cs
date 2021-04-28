using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Kalantyr.Rss.Controllers
{
    [Route("/{controller}")]
    [ApiController]
    public class RssController : ControllerBase
    {
        private readonly IRssService _rssService;

        public RssController(IRssService rssService)
        {
            _rssService = rssService ?? throw new ArgumentNullException(nameof(rssService));
        }

        [HttpGet]
        [Route("feeds")]
        public async Task<ActionResult<string>> GetFeedsAsync(CancellationToken cancellationToken)
        {
            var feeds = await _rssService.GetFeedsAsync(cancellationToken);
            return Ok(feeds.FirstOrDefault());
        }
    }
}
