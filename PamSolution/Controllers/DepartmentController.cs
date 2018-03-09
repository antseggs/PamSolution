using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PamSolution.Controllers
{
    public class DepartmentAddOrEdit
    {
        public string SessionKey { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }

    public class DepartmentType
    {
        public int DepartmentId { get; set; }
        public string DepartmentName{ get; set; }
    }

    public class DepartmentController : ApiController
    {
        //This controller is not complete!
        // This controller should provide CRUD access to the departments 
        // Read - Admin only (Tick)
        // Edit - Admin only (Tick)
        // Create - Admin only (Tick)
        // Delete - Admin only 

        // POST api/department/getAll
        [HttpPost, Route("api/department/getAll")]
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
                            List<DepartmentType> userList = new List<DepartmentType>();
                            userList = ctx.Database.SqlQuery<DepartmentType>("SELECT * FROM department").ToList();
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

        // POST api/department
        public string Post([FromBody]string value)
        {
            // Check user is logged in and session is vaild
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    DepartmentAddOrEdit postUser = JsonConvert.DeserializeObject<DepartmentAddOrEdit>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // check the user has permissions to add a user (Admin only) 
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            // check if id is blank, if user is blank then add the user to the table
                            if (postUser.DepartmentId == -1)
                            {
                                //Create new user!
                                string sql = "INSERT INTO department (departmentName) VALUES ('" + postUser.DepartmentName+ "');";
                                ctx.Database.ExecuteSqlCommand(sql);
                                returnValue = "Pass!";
                            }
                            else
                            {
                                // ELSE update the user.
                                string sql = "UPDATE users SET departmentName = '" + postUser.DepartmentName + "' WHERE departmentId = " + postUser.DepartmentId + ";";
                                ctx.Database.ExecuteSqlCommand(sql);
                                returnValue = "Pass!";
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

        // POST api/department/delete
        [HttpPost, Route("api/department/delete")]
        public string Delete([FromBody]string value)
        {
            // check if the user is logged in and session is valid
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    DepartmentAddOrEdit postUser = JsonConvert.DeserializeObject<DepartmentAddOrEdit>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // check the user has permissions to add a user (Admin only) 
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            // if both yes then run sql delete command on the ID passed into the controller.
                            ctx.Database.ExecuteSqlCommand("DELETE FROM serverAccessLevel WHERE departmentId = " + postUser.DepartmentId + ";");
                            ctx.Database.ExecuteSqlCommand("DELETE FROM users WHERE departmentId = " + postUser.DepartmentId + ";");
                            ctx.Database.ExecuteSqlCommand("DELETE FROM department WHERE departmentId = " + postUser.DepartmentId + ";");
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
