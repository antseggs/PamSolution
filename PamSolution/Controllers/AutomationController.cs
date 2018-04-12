using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PamSolution.Controllers
{
    public class AutomationController : ApiController
    {
        //This controller is not complete!
        // This controller should get all automations the loged in user can run
        // no extra functionality is required at this point but will likely be added at a later date to expand the system.

        // POST api/automation/getAll
        [HttpPost, Route("api/automation/getAll")]
        public List<automationScript> GetAll([FromBody]string value)
        {
            //Get the information from the application
            List<automationScript> returnValue = new List<automationScript>();

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    GetAccessLevelUser postUser = JsonConvert.DeserializeObject<GetAccessLevelUser>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        //Return Json List of users
                        List<automationScript> automationList = new List<automationScript>();
                        automationList = ctx.Database.SqlQuery<automationScript>("SELECT * FROM automationScript WHERE serverOsId = " + postUser.Id + ";").ToList();
                        returnValue = automationList;
                    }
                }

            }
            catch (Exception)
            {
                //returnValue = "Failed! - Exception - " + e;
            }
            return returnValue;
        }
    }
}
