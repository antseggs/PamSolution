using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace PamSolution.Controllers
{
    public class AuthenticationJson
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AuthenticationController : ApiController
    {
        //This controller should only return an authentication, soo..... 
        //This should take a post request of the username and password sent over https then return a random string which has been entered into the sessions table or rerurn "FAIL"
        //Delete function will remain and will be run with no params this will delete any fields old than 1 day from the table (This could then be run automatically)

            // This function still requires input sanitation!
            // UPDATE LOGIN TIME ON USER!
            // Delete any existing sessions for the user!

        // POST api/Authenticate
        public string Post([FromBody]string value)
        {
            // parse the string that is passed in.
            string returnValue = "fail";

            //convert json
            AuthenticationJson postUser = JsonConvert.DeserializeObject<AuthenticationJson>(value);

            try
            {
                // GET THE USERNAME & PASSWORD FROM THE STRING PASSED IN


                // ATTEMPT 1 - USING THE EF connection

                //using (var context = new PamProjectEntities2())
                //{
                //    var query = context.users
                //        .Where(s => s.username == username);
                //}

                // ATTEMPT 2 - USING THE SQL STRING IN ENTITY SQL

                using (var ctx = new PamProjectEntities2())
                {
                    var authenitcationUser = ctx.users.SqlQuery("SELECT * FROM users WHERE username LIKE '" + postUser.Username + "';").FirstOrDefault<user>();
                    if (authenitcationUser.password == postUser.Password)
                    {
                        // Create session key using a crypto framework
                        returnValue = "";
                        byte[] randomNumber = new byte[250];
                        RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();
                        Gen.GetBytes(randomNumber);
                        foreach (int x in randomNumber)
                        {
                            returnValue += x;
                        }

                        // create session in sessions table and return the session key
                        string sql = "INSERT INTO activeSessions (sessionToken, expireTime, userId) VALUES ('" + returnValue + "','" + DateTime.Now.AddHours(12) + "','" + authenitcationUser.userId + "');";

                        ctx.Database.ExecuteSqlCommand(sql);
                    }
                }
            }
            catch (Exception e)
            {
                returnValue = "FAIL - EXCEPTION - " + e;
            }
                      
            return returnValue;
            //Delete any old sessions still active?
        }

        // DELETE api/Authentication/Delete
        public void Delete()
        {
            // Delete all records in the session table that are expired!
            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    ctx.Database.ExecuteSqlCommand("DELETE FROM activeSessions WHERE expireTime < '" + DateTime.Now + "'");
                }
            }
            catch (Exception e)
            {
                var ex = e;
                //This line is used for breakpointing. Expand this to write out to a log file when it fails.
            }

        }
    }
}
