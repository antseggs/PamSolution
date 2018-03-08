using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PamSolution.Controllers
{
    public class AccessLogType
    {
        public string SessionKey { get; set; }
        public int StandardAccountId { get; set; }
        public string UserNote { get; set; }
    }

    public class DeleteRecords
    {
        public string SessionKey { get; set; }
        public DateTime RemoveBeforeTime { get; set; }
    }

    public class AccessLogController : ApiController
    {
        //This controller is not complete!
        //This controller should accept new logs from any logged in user. 
        //This controller should delete logs that are older than a date that the controller is given.(ADMIN ONLY)

        [HttpPost, Route("api/accessLog")]
        // POST api/accessLog
        public string Post([FromBody]string value)
        {
            // Check user is logged in and session is vaild
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    AccessLogType postUser = JsonConvert.DeserializeObject<AccessLogType>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // Get userId from userSession
                        // find the permissionLevel of user from the User in database.
                        var userInfo = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE " + userSession.userId + ";").FirstOrDefault<user>();
                        // add all the data to the database.
                        string sql = "INSERT INTO accessLog (userId, accessTime, standardAccountId, permissionLevelId, userNote) VALUES (" + userInfo.userId + ",'" + DateTime.Now + "'," + postUser.StandardAccountId + "," + userInfo.permissionLevelId + ",'" + postUser.UserNote + "');";
                        ctx.Database.ExecuteSqlCommand(sql);
                        returnValue = "Pass!";
                    }
                }
            }
            catch (Exception e)
            {
                returnValue = "Failed! - Exception - " + e;
            }
            return returnValue;
        }

        [HttpPost, Route("api/accessLog/delete")]
        public string Delete([FromBody]string value)
        {
            // check if the user is logged in and session is valid
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    DeleteRecords postUser = JsonConvert.DeserializeObject<DeleteRecords>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // check the user has permissions to add a user (Admin only) 
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            // if both yes then run sql delete command on the date passed into the controller.
                            ctx.Database.ExecuteSqlCommand("DELETE FROM accessLog WHERE accessTime <= '" + postUser.RemoveBeforeTime + "';");
                            returnValue = "Pass!";
                        }
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
