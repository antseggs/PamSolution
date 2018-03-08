using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PamSolution.Controllers
{
    public class AccountGroupType
    {
        public string SessionKey { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public int PermissionLevels { get; set; }
        public int AccountGroupId { get; set; }
    }

    public class AccountGroupController : ApiController
    {
        //This controller is not complete!

        // This controller needs to provide CRUD access to the group table in the database.
        // All parts of this controller (except read) require admin access.

        // POST api/accountGroups/getAll
        [HttpPost, Route("api/accountGroups/GetAll")]
        public string GetAll([FromBody]string value)
        {
            //Get the information from the application
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    UserGetList postGroup = JsonConvert.DeserializeObject<UserGetList>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postGroup.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        //Return Json List of accountGroups
                        List<accountGroup> userList = new List<accountGroup>();
                        userList = ctx.Database.SqlQuery<accountGroup>("SELECT * FROM accountGroup").ToList();
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

        // POST api/accountGroups/delete
        [HttpPost, Route("api/accountGroups/delete")]
        public string Delete([FromBody]string value)
        {
            // check if the user is logged in and session is valid
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    AccountGroupType postGroup = JsonConvert.DeserializeObject<AccountGroupType>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postGroup.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // check the user has permissions to add a user (Admin only) 
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            // if both yes then run sql delete command on the ID passed into the controller.
                            ctx.Database.ExecuteSqlCommand("DELETE FROM standardAccount WHERE accountGroupId = " + postGroup.AccountGroupId + ";");
                            ctx.Database.ExecuteSqlCommand("DELETE FROM protectedAccount WHERE accountGroupId = " + postGroup.AccountGroupId + ";");
                            ctx.Database.ExecuteSqlCommand("DELETE FROM accountGroup WHERE accountGroupId = " + postGroup.AccountGroupId + ";");
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


        // POST api/accountGroups
        [HttpPost, Route("api/accountGroups")]
        public string Post([FromBody]string value)
        {
            // Check user is logged in and session is vaild
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    AccountGroupType postGroup = JsonConvert.DeserializeObject<AccountGroupType>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postGroup.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // check the user has permissions to add a user (Admin only) 
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            // check if id is blank, if user is blank then add the group to the table
                            if (postGroup.AccountGroupId == -1)
                            {
                                //Create new user!
                                string sql = "INSERT INTO users (groupName, groupDescription, permissionLevels) VALUES ('" + postGroup.GroupName + "','" + postGroup.GroupDescription + "','" + postGroup.PermissionLevels + "');";
                                ctx.Database.ExecuteSqlCommand(sql);
                                returnValue = "Passed!";
                            }
                            else
                            {
                                // ELSE update the user.
                                string sql = "UPDATE users SET groupName = '" + postGroup.GroupName + "', groupDescription = '" + postGroup.GroupDescription + "', permissionLevels = '" + postGroup.PermissionLevels + "' WHERE accountGroupId = " + postGroup.AccountGroupId + ";";
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
