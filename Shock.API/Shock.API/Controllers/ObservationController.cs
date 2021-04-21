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
    [Route("api/[controller]/v1")]
    public class ObservationController : ControllerBase
    {
        private ObservationDbContext dbContext;

        public ObservationController(ObservationDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        // GET: api/Observation
        [HttpGet("GetAllObservationsForUser")]
        public IEnumerable<Observation> Get(string user)
        {
            return user.ToLower()=="admin"?dbContext.Observations: dbContext.Observations.Where(a=>a.CreatedUser.ToLower()==user.ToLower());
        }

        [HttpGet("GetAllObservations")]
        public IEnumerable<Observation> GetAllObservation()
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
        [HttpPost("AddObservation")]
        public IActionResult Post([FromBody]Observation observation)
        {
            observation.Id = Guid.NewGuid();
            observation.DateCreated = DateTime.UtcNow;
            observation.DateModified= DateTime.UtcNow;
            var data = dbContext.Observations.ToList();
            var count = data.Count() != 0 ? data.Max(a=>a.RefNo) : 0;

            observation.RefNo = count + 1;
            try
            {
                dbContext.Observations.Add(observation);
                dbContext.SaveChanges();
                return new OkObjectResult(observation);
            }
            catch
            {
                return BadRequest("Something went wrong, Details not added");
            }
        }

        // PUT: api/Quality/5
        [HttpPost("UpdateObservation")]
        public IActionResult Update([FromBody]Observation observation)
        {
            try
            {
                var res = dbContext.Observations.Where(a => a.Id == observation.Id).FirstOrDefault();

                if (res != null)
                {
                    res.ProjectLocation = observation.ProjectLocation;
                    res.Name = observation.Name;
                    res.ActionOwner = observation.ActionOwner;
                    res.AgreedAction = observation.AgreedAction;
                    res.Category = observation.Category;
                    res.CreatedUser = observation.CreatedUser;
                    res.Description = observation.Description;
                    res.DueDate = observation.DueDate;
                    res.Evidence = observation.Evidence;
                    res.ProjectName = observation.ProjectName;
                    res.Status = observation.Status;
                    res.TypeOfActivity = observation.TypeOfActivity;
                    res.DateModified = DateTime.UtcNow;
                    dbContext.Observations.Update(res);
                    dbContext.SaveChanges();
                }
                return new OkObjectResult(res);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong, item not updated" + ex.Message);
            }

        }

        [HttpPost("FilterObservation")]
        public IActionResult GetObservationsWithFilter(FilterModel filterModel)
        {
            try
            {
                var response = dbContext.Observations.Where(a => a.DueDate.Date >= filterModel.FromDate.Date
                            && a.DueDate.Date <= filterModel.ToDate.Date);

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong, item not updated" + ex.Message);
            }
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("DeleteObservation")]
        public IActionResult Delete(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    Guid.TryParse(id, out Guid result);

                    var res = dbContext.Observations.Where(a => a.Id == result).FirstOrDefault();

                    if (res != null)
                    {
                        dbContext.Observations.Remove(res);
                        dbContext.SaveChanges();
                    }
                    return new OkResult();
                }
                else
                {
                    return BadRequest("Something went wrong, item not deleted");
                }
            }
            catch
            {
                return BadRequest("Something went wrong, item not deleted");
            }
        }
    }
}
