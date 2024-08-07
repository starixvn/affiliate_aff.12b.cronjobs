using Fanex.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Helpers
{
    public class HttpLoggingHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        public HttpLoggingHandler() : this(null)
        {
        }

        public HttpLoggingHandler(ILogger logger)
        {
            _logger = logger ?? Logger.Log;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var contentRequest = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            await _logger.InfoAsync($"HttpMethod: {request.Method.Method}, HttpContent:{contentRequest}, Content-Type:{JsonConvert.SerializeObject(request.Content.Headers.ContentType)}");
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            await _logger.InfoAsync($"HttpStatusCode: {response.StatusCode}, HttpContent:{content}");

            return response;
        }
    }
}
