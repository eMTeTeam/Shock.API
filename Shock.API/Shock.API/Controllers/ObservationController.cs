using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shock.API.Data;
using Shock.API.DataModel;
using System.Net;
using System.Net.Mail;

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
                _ = SendMail(observation, true);
                return new OkObjectResult(observation);
            }
            catch
            {
                return BadRequest("Something went wrong, Details not added");
            }
        }

        [HttpPost("SendMail")]
        public async Task SendMail(Observation observation, bool isUpdate)
        {
            var fromAddress = new MailAddress("shocobserver@gmail.com", "SHOC ADMIN");
            var toAddress = new MailAddress("shocuser@gmail.com", "SHOC USERS");
            const string fromPassword = "Shoc@123";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            var color = observation.Status == "Open" ? "red" : observation.Status == "Inprogress" ? "yellow" : "green";
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                IsBodyHtml = true,
                Subject = isUpdate == true ? "Observation updated" : "New Observation added",
                Body = " <br/> <b>Observation Name:</b> " + observation.Name + "<br/>" +
                "<b>Project Name:</b> " + observation.ProjectName + "<br/>" +
               //  "<b>Status :</b> <span style='color:"+color+"'" + observation.Status + "</span><br/>" +
               "<b>Status :" + observation.Status + "<br/>" +
                "<b>Project Location :" + observation.ProjectLocation + "<br/>" +
                "<b>Activity:</b> " + observation.TypeOfActivity + "<br/>" +
                "<b>Action Owner:</b> " + observation.ActionOwner + "<br/>" +
                 "<b>Agreed Action:</b> " + observation.AgreedAction + "<br/>" +
                "<b>Category:</b> " + observation.Category + "<br/>" +
                "<b>Created User: " + observation.CreatedUser + "<br/>" +
                "<b>Created Date:</b> " + observation.DateCreated + "<br/>" +
                "<b>Description:</b> " + observation.Description + "<br/>" +
                 "<b>Evidence:</b> " + observation.Evidence + "<br/>" +
                 "<br/>" +
                "<b>App Link:</b> <a href='https://shock-ui.herokuapp.com' target='_blank'> https://shock-ui.herokuapp.com</a> <br/>",
            })

            {
               // message.To.Add("");
                //message.To.Add("shocuser@gmail.com");
                await smtp.SendMailAsync(message);
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
                _ = SendMail(observation, true);
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
