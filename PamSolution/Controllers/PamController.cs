using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PamSolution.Controllers
{
    public class PamController : Controller
    {
        // GET: Pam
        public ActionResult Index()
        {
            return View();
        }


        public string[] Get()
        {
            return new string[]
                {
                    "Hello",
                    "World"
                };
        }
    }
}