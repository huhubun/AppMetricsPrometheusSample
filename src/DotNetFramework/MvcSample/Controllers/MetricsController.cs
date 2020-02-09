using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcSample.Controllers
{
    public class MetricsController : Controller
    {
        [Route("metrics")]
        public async Task<ActionResult> GetMetrics()
        {
            var formatter = new App.Metrics.Formatters.Prometheus.MetricsPrometheusTextOutputFormatter();
            var snapshot = MvcMetrics.GetMetrics().Snapshot.Get();

            using (var ms = new MemoryStream())
            {
                await formatter.WriteAsync(ms, snapshot);
                var result = Encoding.UTF8.GetString(ms.ToArray());

                return Content(result, formatter.MediaType.ContentType, Encoding.UTF8);
            }
        }
    }
}