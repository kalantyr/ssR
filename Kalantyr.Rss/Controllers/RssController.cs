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
        private readonly IInvokeLogger _invokeLogger;

        public RssController(IRssService rssService, IInvokeLogger invokeLogger)
        {
            _rssService = rssService ?? throw new ArgumentNullException(nameof(rssService));
            _invokeLogger = invokeLogger ?? throw new ArgumentNullException(nameof(invokeLogger));
        }

        [HttpGet]
        [Route("feeds")]
        public ActionResult<string[]> GetFeedIds(string id, CancellationToken cancellationToken)
        {
            try
            {
                var feed = _rssService.GetFeedIds(id);
                return Ok(feed);
            }
            catch (Exception e)
            {
                return base.StatusCode((int)HttpStatusCode.InternalServerError, e.GetBaseException().Message);
            }
        }

        [HttpGet]
        [Route("feed/{id}")]
        public async Task<ActionResult<Feed>> GetFeedAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                _invokeLogger.Log();

                var feed = await _rssService.GetFeedAsync(id, cancellationToken);
                return Ok(feed);
            }
            catch (Exception e)
            {
                return base.StatusCode((int)HttpStatusCode.InternalServerError, e.GetBaseException().Message);
            }
        }

        [HttpGet]
        [Route("t11/feed/{id}")]
        public async Task<ActionResult<Feed>> GetTestFeedAsync(string id, CancellationToken cancellationToken)
        {
            return await GetFeedAsync(id, cancellationToken);
        }

        [HttpGet]
        [Route("statistics")]
        public ActionResult<string[]> GetStatistics(string id, CancellationToken cancellationToken)
        {
            try
            {
                if (_invokeLogger is IStatistics statistics)
                    return Ok(statistics.GetStatistics());
                    
                return Ok(new string[0]);
            }
            catch (Exception e)
            {
                return base.StatusCode((int)HttpStatusCode.InternalServerError, e.GetBaseException().Message);
            }
        }
    }
}
