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
        public string GetAll([FromBody]string value)
        {
            //Get the information from the application
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    UserGetList postUser = JsonConvert.DeserializeObject<UserGetList>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        //Get user access level    
                        serverAccessLevel currentUserLevel = new serverAccessLevel();
                        currentUserLevel = ctx.Database.SqlQuery<serverAccessLevel>("SELECT * FROM serverAccessLevel WHERE usersId = " + userSession.userId + ";").FirstOrDefault();

                        //Return Json List of users
                        List<UserGeneral> userList = new List<UserGeneral>();
                        userList = ctx.Database.SqlQuery<UserGeneral>("SELECT * FROM automationScript WHERE serverAccessLevelId = " + currentUserLevel.serverAccessId + ";").ToList();
                        returnValue = JsonConvert.SerializeObject(userList);
                    }
                }

            }
            catch (Exception e)
            {
                returnValue = "Failed! - Exception - " + e;
            }
            return returnValue;
        }
    }
}
