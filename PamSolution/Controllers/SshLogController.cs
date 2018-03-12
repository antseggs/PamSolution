using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PamSolution.Controllers
{
    public class SshLogType
    {
        public string SessionKey { get; set; }
        public int UserId { get; set; }
        public string LogContent { get; set; }
        public int PermissionLevelId { get; set; }
        public DateTime FinishTime { get; set; }
        public string UserNote { get; set; }
        public int ProtectedAccountId { get; set; }
    }

    public class SshLogController : ApiController
    {
        [HttpPost, Route("api/sshLog")]
        // POST api/sshLog
        public string Post([FromBody]string value)
        {
            // Check user is logged in and session is vaild
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    SshLogType postUser = JsonConvert.DeserializeObject<SshLogType>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // Get userId from userSession
                        // find the permissionLevel of user from the User in database.
                        var userInfo = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE " + userSession.userId + ";").FirstOrDefault<user>();
                        // add all the data to the database.
                        string sql = "INSERT INTO sshLog (userId, accessTime, logContent, permissionLevelId, userNote, protectedAccountId) VALUES (" + userInfo.userId + ",'" + DateTime.Now + "','" + postUser.LogContent + "'," + userInfo.permissionLevelId + ",'" + postUser.UserNote + "', " + postUser.ProtectedAccountId + ");";
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

        [HttpPost, Route("api/sshLog/delete")]
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
                            ctx.Database.ExecuteSqlCommand("DELETE FROM sshLog WHERE accessTime <= '" + postUser.RemoveBeforeTime + "';");
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
