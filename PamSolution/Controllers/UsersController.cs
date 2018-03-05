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

    public class UserGetList
    {
        public string SessionKey { get; set; }
    }

    public class UsersController : ApiController
    {
        //This controller is not complete!
        //This controller should be used to see if a user is logged in, if they are then perform CRUD against users in the system Either reading a list of users (if user is admin)
        //delete a user from the system, update a users details or create a new user in the system.

        // GET api/Users
        [HttpPost, Route("api/users/getAll")]
        public string Get([FromBody]string value)
        {
            //Get the information from the application
            string returnValue = "fail";
            try
            {
                //UserGetList postUser = JsonConvert.DeserializeObject<UserGetList>(value);


            }catch (Exception e)
            {
                returnValue = "Failed! - Exception - " + e;
            }
            return returnValue;
        }   

        // GET api/users/
        public string Get(int id)
        {
            return "value";
        }

        // POST api/users
        public void Post([FromBody]string value)
        {
            // Check user is logged in and session is vaild
            // check the user has permissions to add a user (Admin only) 
            // check if id is blank, if user is blank then add the user to the table
            // ELSE update the user.
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            // check if the user is logged in and session is valid
            // check user is admin
            // if both yes then run sql delete command on the ID passed into the controller.
        }
    }
}
