﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PamSolution.Controllers
{
    public class GetAccessLevelUser
    {
        public string SessionKey { get; set; }
        public int Id { get; set; }
    }

    public class ServerAccessType
    {
        public string SessionKey { get; set; }
        public int ServerAccessId { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public int ServerId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public int? StandardAccountId { get; set; }
        public bool Allowed { get; set; }
    }

    public class ServerAccess
    {
        public int ServerAccessId { get; set; }
        public int? UserId { get; set; }
        public int? DepartmentId { get; set; }
        public int? ServerId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public int? StandardAccountId { get; set; }
        public bool? Allowed { get; set; }
    }

    public class ServerAccessLevelController : ApiController
    {
        // POST api/protectedAccount/getall
        [HttpPost, Route("api/protectedAccount/getAll")]
        public List<ServerAccess> GetAll([FromBody]string value)
        {
            //Get the information from the application Get all access for a user.
            //string returnValue = "fail";
            List<ServerAccess> returnValue = new List<ServerAccess>();

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    GetAccessLevelUser postUser = JsonConvert.DeserializeObject<GetAccessLevelUser>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        //Is user admin?
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            //Return Json List of levels
                            List<ServerAccess> levelList = new List<ServerAccess>();
                            levelList = ctx.Database.SqlQuery<ServerAccess>("SELECT ServerAccessId, userId, departmentId, serverId, startTime, finishTime, standardAccountId, allowed FROM serverAccessLevel WHERE userId = " + postUser.Id + ";").ToList();
                            returnValue = levelList;
                        }else
                        {
                            if (accessUser.userId == postUser.Id)
                            {
                                //Return Json List of levels
                                List<ServerAccess> levelList = new List<ServerAccess>();
                                levelList = ctx.Database.SqlQuery<ServerAccess>("SELECT ServerAccessId, userId, departmentId, serverId, startTime, finishTime, standardAccountId, allowed FROM serverAccessLevel WHERE userId = " + postUser.Id + ";").ToList();
                                returnValue = levelList;
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                //returnValue = "Failed! - Exception - " + e;
                int i = 1;//Used so can breakpoint
            }
            return returnValue;
        }

        // POST api/serverAccessLevel/delete
        [HttpPost, Route("api/serverAccessLevel/delete")]
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
                            ctx.Database.ExecuteSqlCommand("DELETE FROM serverAccessLevel WHERE serverAccessId = " + postUser.Id + ";");
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

        // POST api/serverAccessLevel
        public string Post([FromBody]string value)
        {
            // Check user is logged in and session is vaild
            string returnValue = "fail";

            try
            {
                using (var ctx = new PamProjectEntities2())
                {
                    ServerAccessType postUser = JsonConvert.DeserializeObject<ServerAccessType>(value);
                    //Is session active?
                    var userSession = ctx.activeSessions.SqlQuery("SELECT * FROM activeSessions WHERE sessionToken LIKE '" + postUser.SessionKey + "';").FirstOrDefault<activeSession>();
                    if (userSession.expireTime >= DateTime.Now)
                    {
                        // check the user has permissions to add a user (Admin only) 
                        var accessUser = ctx.users.SqlQuery("SELECT * FROM users WHERE userId LIKE '" + userSession.userId + "';").FirstOrDefault<user>();
                        if (accessUser.permissionLevelId == 1 || accessUser.permissionLevelId == 2)
                        {
                            // check if id is blank, if user is blank then add the user to the table
                            if (postUser.ServerAccessId == -1)
                            {
                                //Create new user!
                                string sql = "INSERT INTO serverAccessLevel (userId, departmentId, serverId, startTime, finishTime, standardAccountId, allowed) VALUES (" + postUser.UserId + "," + postUser.DepartmentId + "," + postUser.ServerId + ",'" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "','" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "', " + (postUser.StandardAccountId ?? SqlInt32.Null)+ ", 1);";
                                ctx.Database.ExecuteSqlCommand(sql);
                                returnValue = "Passed!";
                            }
                            else
                            {
                                // ELSE update the user.
                                string sql = "UPDATE serverAccessLevel SET userId = " + postUser.UserId + ", departmentId = " + postUser.DepartmentId + ", serverId = " + postUser.ServerId + ", startTime = '" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "', finishTime = '" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "', standardAccountId = " + postUser.StandardAccountId + ", allowed = 1 WHERE serverAccessId = " + postUser.ServerAccessId + ";";
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
