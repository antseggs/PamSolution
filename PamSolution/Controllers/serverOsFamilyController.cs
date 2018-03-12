using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PamSolution.Controllers
{
    public class ServerOsFamilyController : ApiController
    {
        //This controller is not complete!
        //This is complete as there is currently no need to have any CRUD to this table but this may be 
        // implemented in the future if needed.

        // POST api/osFamily/getAll
        [HttpPost, Route("api/osFamily/getAll")]
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
                            //Return Json List of users
                            List<serverOsFamily> userList = new List<serverOsFamily>();
                            userList = ctx.Database.SqlQuery<serverOsFamily>("SELECT * FROM serverOsFamily").ToList();
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
