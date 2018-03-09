using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PamSolution.Controllers
{
    public class UserEditOrAdd
    {
        public string SessionKey { get; set; }
        public int UserId { get; set; }
        public int PermissionLevelId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string JobTitle { get; set; }
        public int DepartmentId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string Note { get; set; }
    }

    public class UserGeneral
    {
        public int UserId { get; set; }
        public int PermissionLevelId { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string JobTitle { get; set; }
        public int DepartmentId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string Note { get; set; }
    }

    public class UserGetList
    {
        public string SessionKey { get; set; }
    }

    public class UsersController : ApiController
    {
        //This controller is not complete!
        //This controller should be used to see if a user is logged in, if they are then perform CRUD against users in the system Either reading a list of users (if user is admin)
        //delete a user from the system, update a users details or create a new user in the system.

        // POST api/Users/getAll
        [HttpPost, Route("api/users/getAll")]
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
                        //Is user admin?
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            //Return Json List of users
                            List<UserGeneral> userList = new List<UserGeneral>();
                            userList = ctx.Database.SqlQuery<UserGeneral>("SELECT * FROM users").ToList();
                            returnValue = JsonConvert.SerializeObject(userList);
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

        [HttpPost, Route("api/users/getSelf")]
        // POST api/users/getSelf
        public string getSelf([FromBody]string value)
        {
            // Check user is logged in
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
                        // Return there own user details
                        UserGeneral self = ctx.Database.SqlQuery<UserGeneral>("SELECT * FROM users WHERE userId = " + userSession.userId +";").FirstOrDefault<UserGeneral>();
                        returnValue = JsonConvert.SerializeObject(self);
                    }
                }
            }
            catch (Exception e)
            {
                returnValue = "Failed! - Exception - " + e;
            }
            return returnValue;
        }

        // POST api/users
        public string Post([FromBody]string value)
        {
            // Check user is logged in and session is vaild
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    UserEditOrAdd postUser = JsonConvert.DeserializeObject<UserEditOrAdd>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // check the user has permissions to add a user (Admin only) 
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            // check if id is blank, if user is blank then add the user to the table
                            if (postUser.UserId == -1)
                            {
                                //Create new user!
                                string sql = "INSERT INTO users (permissionLevelId, firstName, surname, jobTitle, departmentId, username, password, lastLoginDate, note) VALUES (" + postUser.PermissionLevelId + ",'" + postUser.FirstName + "','" + postUser.Surname + "','" + postUser.JobTitle + "'," + postUser.DepartmentId + ",'" + postUser.Username + "','" + postUser.Password + "','" + postUser.LastLoginDate + "','" + postUser.Note + "');";
                                ctx.Database.ExecuteSqlCommand(sql);
                                returnValue = "Passed!";
                                //CHECK FOR USERNAME CLASHES!!!!
                            }
                            else
                            {
                                // ELSE update the user.
                                string sql = "UPDATE users SET permissionLevelId =" + postUser.PermissionLevelId + ", firstName = '" + postUser.FirstName + "', surname = '" + postUser.Surname + "', jobtitle = '" + postUser.JobTitle + "', departmentId = " + postUser.DepartmentId + ", username = '" + postUser.Username + "', password = '" + postUser.Password + "', lastLoginDate = '" + postUser.LastLoginDate + "', note = '" + postUser.Note + "' WHERE userId = " + postUser.UserId + ";";
                                ctx.Database.ExecuteSqlCommand(sql);
                                //CHECK FOR USERNAME CLASHES!!!!
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

        // POST api/users/delete
        [HttpPost, Route("api/users/delete")]
        public string Delete([FromBody]string value)
        {
            // check if the user is logged in and session is valid
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    UserEditOrAdd postUser = JsonConvert.DeserializeObject<UserEditOrAdd>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // check the user has permissions to add a user (Admin only) 
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            // if both yes then run sql delete command on the ID passed into the controller.
                            ctx.Database.ExecuteSqlCommand("DELETE FROM sshLog WHERE userId = " + postUser.UserId + ";");
                            ctx.Database.ExecuteSqlCommand("DELETE FROM desktopLog WHERE userId = " + postUser.UserId + ";");
                            ctx.Database.ExecuteSqlCommand("DELETE FROM accessLog WHERE userId = " + postUser.UserId + ";");
                            ctx.Database.ExecuteSqlCommand("DELETE FROM serverAccessLevel WHERE userId = " + postUser.UserId + ";");
                            ctx.Database.ExecuteSqlCommand("DELETE FROM activeSessions WHERE userId = " + postUser.UserId + ";");
                            ctx.Database.ExecuteSqlCommand("DELETE FROM users WHERE userId = " + postUser.UserId + ";");
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
