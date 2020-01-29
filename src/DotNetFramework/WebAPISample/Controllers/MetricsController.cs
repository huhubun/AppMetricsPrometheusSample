using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebAPISample.Controllers
{
    [RoutePrefix("metrics")]
    public class MetricsController : ApiController
    {
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetMetricsAsync()
        {
            var formatter = new App.Metrics.Formatters.Prometheus.MetricsPrometheusTextOutputFormatter();
            var snapshot = ApiMetrics.GetMetrics().Snapshot.Get();

            using (var ms = new MemoryStream())
            {
                await formatter.WriteAsync(ms, snapshot);
                var result = Encoding.UTF8.GetString(ms.ToArray());

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result, Encoding.UTF8, formatter.MediaType.ContentType);

                return response;
            }
        }
    }
}
