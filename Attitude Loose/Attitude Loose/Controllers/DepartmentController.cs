using Attitude_Loose.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Attitude_Loose.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private IDepartmentService departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            this.departmentService = departmentService;
        }

        public ActionResult GetDepartmentList()
        {
            var reportlist = departmentService.GetDepartments().Select(x => new { DepartmentName = x.DepartmentName }).OrderBy(x => x.DepartmentName).ToList();
            return Json(reportlist, JsonRequestBehavior.AllowGet);
        }
    }
}