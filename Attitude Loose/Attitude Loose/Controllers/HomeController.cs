using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Attitude_Loose.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [OutputCache(Duration = 10, VaryByParam = "none")]
        public ActionResult Index()
        {
            if (TempData["AlertMessage"] == null)
            {
                TempData["AlertMessage"] = "";
            }
            return View();
        }

        [HandleError(View = "Error")]
        public ActionResult About()
        {
            int test = 0;
            int b = 1 / test;
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}