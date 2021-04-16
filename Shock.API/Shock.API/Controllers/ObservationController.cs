using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shock.API.Data;
using Shock.API.DataModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Shock.API.Controllers
{
    [Route("api/[controller]")]
    public class ObservationController : ControllerBase
    {
        private ObservationDbContext dbContext;

        public ObservationController(ObservationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        // GET: api/Observation
        [HttpGet("GetAllObservations")]
        public IEnumerable<Observation> Get()
        {
            return dbContext.Observations;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
