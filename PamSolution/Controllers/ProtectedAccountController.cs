using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PamSolution.Controllers
{
    public class GetAccountForServer
    {
        public string SessionKey { get; set; }
        public int Id { get; set; }
    }

    public class AddEditProtectedAccount
    {
        public string SessionKey { get; set; }
        public int ProtectedAccountId { get; set; }
        public string AccountName { get; set; }
        public int ServerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int AccountGroupId { get; set; }
        public string Note { get; set; }
    }

    public class ProtectedAccountController : ApiController
    {
        // This controller needs to be able to 
        // Get an account for a server available to that user. (TICK)
        // edit a account (if admin) (Tick)
        // Add an acocunt (if admin) (Tick)
        // delete an account (if admin) (TICK)

        // POST api/protectedAccount/get
        [HttpPost, Route("api/protectedAccount/get")]
        public string GetAll([FromBody]string value)
        {
            //Get the information from the application
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    GetAccountForServer postUser = JsonConvert.DeserializeObject<GetAccountForServer>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {      
                        //Return Json account details
                        returnValue = JsonConvert.SerializeObject(ctx.Database.SqlQuery<protectedAccount>("SELECT * FROM protectedAccount WHERE serverId = " + postUser.Id).FirstOrDefault());
                    }
                }

            }
            catch (Exception e)
            {
                returnValue = "Failed! - Exception - " + e;
            }
            return returnValue;
        }

        // POST api/protectedAccount/delete
        [HttpPost, Route("api/protectedAccount/delete")]
        public string Delete([FromBody]string value)
        {
            // check if the user is logged in and session is valid
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    GetAccountForServer postUser = JsonConvert.DeserializeObject<GetAccountForServer>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // check the user has permissions to add a user (Admin only) 
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            // if both yes then run sql delete command on the ID passed into the controller.
                            ctx.Database.ExecuteSqlCommand("DELETE FROM protectedAccount WHERE protectedAccountId = " + postUser.Id + ";");
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

        // POST api/protectedAccount
        public string Post([FromBody]string value)
        {
            // Check user is logged in and session is vaild
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    AddEditProtectedAccount postUser = JsonConvert.DeserializeObject<AddEditProtectedAccount>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // check the user has permissions to add a user (Admin only) 
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            // check if id is blank, if user is blank then add the user to the table
                            if (postUser.ProtectedAccountId == -1)
                            {
                                //Create new user!
                                string sql = "INSERT INTO protectedAccount (accountName, serverId, username, password, accountGroupId, note) VALUES ('" + postUser.AccountName + "'," + postUser.ServerId + ",'" + postUser.Username + "','" + postUser.Password + "'," + postUser.AccountGroupId + ",'" + postUser.Note + "');";
                                ctx.Database.ExecuteSqlCommand(sql);
                                returnValue = "Passed!";
                            }
                            else
                            {
                                // ELSE update the user.
                                string sql = "UPDATE protectedAccount SET accountName = '" + postUser.AccountName + "', serverId = " + postUser.ServerId + ", username = '" + postUser.Username + "', password = '" + postUser.Password + "', accountGroupId = " + postUser.AccountGroupId + ", note = '" + postUser.Note + "' WHERE protectedAccountId = " + postUser.ProtectedAccountId + ";";
                                ctx.Database.ExecuteSqlCommand(sql);
                                returnValue = "Passed!";
                            }
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
