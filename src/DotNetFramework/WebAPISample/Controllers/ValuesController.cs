using System.Collections.Generic;
using System.Web.Http;
using static WebAPISample.RandomThings;

namespace WebAPISample.Controllers
{
    [RoutePrefix("api/values")]
    public class ValuesController : ApiController
    {
        // GET api/values
        [HttpGet, Route("")]
        public IEnumerable<string> Get()
        {
            RandomSleep(Speed.High);
            RandomCrash();

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet, Route("{id:int}")]
        public string Get([FromUri]int id)
        {
            RandomSleep(Speed.High);
            RandomCrash();

            return "value" + id;
        }

        // POST api/values
        [HttpPost, Route("")]
        public void Post([FromBody]string value)
        {
            RandomSleep(Speed.Slow);
            RandomCrash();
        }

        // PUT api/values/5
        [HttpPut, Route("{id:int}")]
        public void Put([FromUri]int id, [FromBody]string value)
        {
            RandomSleep(Speed.Slow);
            RandomCrash();
        }

        // DELETE api/values/5
        [HttpDelete, Route("{id:int}")]
        public void Delete([FromUri]int id)
        {
            RandomSleep(Speed.High);
            RandomCrash();
        }
    }
}
