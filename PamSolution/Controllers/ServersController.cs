using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PamSolution.Controllers
{
    public class ServerType
    {
        public string SessionKey { get; set; }
        public int ServerId { get; set; }
        public int ServerOsId { get; set; }
        public string ServerName { get; set; }
        public string ServerDescription { get; set; }
        public string ServerIp { get; set; }
        public string Fqdn { get; set; }
        public string ServerNote { get; set; }
        public bool IpStatic { get; set; }
    }

    public class ServerNoInher
    {
        public int ServerId { get; set; }
        public int ServerOsId { get; set; }
        public string ServerName { get; set; }
        public string ServerDescription { get; set; }
        public string ServerIp { get; set; }
        public string Fqdn { get; set; }
        public string ServerNote { get; set; }
        public bool IpStatic { get; set; }

        public static implicit operator ServerNoInher(DbRawSqlQuery<ServerNoInher> v)
        {
            throw new NotImplementedException();
        }
    }

    public class ServersController : ApiController
    {
        // This controller is not complete
        // This controller should do a few things
        // First it should be able to check a user has correct permissions using the session then add a server, delete a server, edit a server.
        // Next it should return a list of servers and attributes to the application. 

        // POST api/server/get
        [HttpPost, Route("api/servers/get")]
        public ServerNoInher Get([FromBody]string value)
        {
            //Get the information from the application Get all access for a user.
            ServerNoInher returnValue = new ServerNoInher();

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    GetAccessLevelUser postUser = JsonConvert.DeserializeObject<GetAccessLevelUser>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        //Return 
                        //List<ServerNoInher> levelList = new List<ServerNoInher>();
                        //levelList = ctx.Database.SqlQuery<ServerNoInher>("SELECT * FROM server WHERE serverId = " + postUser.Id + ";").ToList();
                        //returnValue = JsonConvert.SerializeObject(levelList);
                        var reter = ctx.Database.SqlQuery<ServerNoInher>("SELECT * FROM server WHERE serverId = " + postUser.Id + ";").FirstOrDefault<ServerNoInher>();
                        returnValue = reter;
                    }
                }

            }
            catch (Exception)
            {
                //returnValue = "Failed! - Exception - " + e;
            }
            return returnValue;
        }

        // POST api/servers/delete
        [HttpPost, Route("api/servers/delete")]
        public string Delete([FromBody]string value)
        {
            // check if the user is logged in and session is valid
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    GetAccessLevelUser postUser = JsonConvert.DeserializeObject<GetAccessLevelUser>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // check the user has permissions to add a user (Admin only) 
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            // if both yes then run sql delete command on the ID passed into the controller.
                            ctx.Database.ExecuteSqlCommand("DELETE FROM server WHERE serverId = " + postUser.Id + ";");
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

        // POST api/servers
        public string Post([FromBody]string value)
        {
            // Check user is logged in and session is vaild
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    ServerType postUser = JsonConvert.DeserializeObject<ServerType>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // check the user has permissions to add a user (Admin only) 
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            // check if id is blank, if user is blank then add the user to the table
                            if (postUser.ServerId == -1)
                            {
                                //Create new user!
                                string sql = "INSERT INTO server (serverName, serverOsId, serverDescription, serverIp, ipStatic, fqdn, serverNote) VALUES ('" + postUser.ServerName + "'," + postUser.ServerOsId + ",'" + postUser.ServerDescription + "','" + postUser.ServerIp + "', " + postUser.IpStatic + ", '" + postUser.Fqdn + "','" + postUser.ServerNote + "');";
                                ctx.Database.ExecuteSqlCommand(sql);
                                returnValue = "Passed!";
                            }
                            else
                            {
                                // ELSE update the user.
                                string sql = "UPDATE serverAccessLevel SET serverName = '" + postUser.ServerName + "', serverOsId = " + postUser.ServerOsId + ", serverDescription = '" + postUser.ServerDescription + "', serverIp = '" + postUser.ServerIp + "', ipStatic = " + postUser.IpStatic + ", fqdn = '" + postUser.Fqdn + "', serverNote = '" + postUser.ServerNote + "' WHERE serverId = " + postUser.ServerId + ";";
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
