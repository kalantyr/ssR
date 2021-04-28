using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Rss.Model;
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
        [Route("t5/feed/{id}")]
        public async Task<ActionResult<Feed>> GetFeedAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                var feed = await _rssService.GetFeedAsync(id, cancellationToken);
                return Ok(feed);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return base.StatusCode((int)HttpStatusCode.InternalServerError, e.GetBaseException().Message);
            }
        }
    }
}
