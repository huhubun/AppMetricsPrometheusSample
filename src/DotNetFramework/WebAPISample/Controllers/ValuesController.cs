using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Http;

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
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet, Route("{id:int}")]
        public string Get([FromUri]int id)
        {
            RandomSleep(Speed.High);
            return "value" + id;
        }

        // POST api/values
        [HttpPost, Route("")]
        public void Post([FromBody]string value)
        {
            RandomSleep(Speed.Slow);
        }

        // PUT api/values/5
        [HttpPut, Route("{id:int}")]
        public void Put([FromUri]int id, [FromBody]string value)
        {
            RandomSleep(Speed.Slow);
        }

        // DELETE api/values/5
        [HttpDelete, Route("{id:int}")]
        public void Delete([FromUri]int id)
        {
            RandomSleep(Speed.High);
        }

        private void RandomSleep(Speed speed)
        {
            const int MIN = 50;
            const int MIDDLE = 200;
            const int LARGE = 1000;

            int time;

            switch (speed)
            {
                case Speed.High:
                    time = new Random().Next(MIN, MIDDLE);
                    break;

                case Speed.Slow:
                    time = new Random().Next(MIDDLE, LARGE);
                    break;

                default:
                    time = new Random().Next(MIN, LARGE);
                    break;
            }

            Thread.Sleep(time);
        }
    }

    public enum Speed
    {
        High,
        Slow
    }
}
