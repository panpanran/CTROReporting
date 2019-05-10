using CTROLibrary.Infrastructure;
using CTROReporting.Service;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CTROReporting.Controllers
{
    [Authorize]
    public class LoggerController : Controller
    {
        private readonly ILoggerService loggerService;
        private readonly IUserService userService;
        private readonly IReportSettingService reportsettingService;

        public LoggerController(ILoggerService loggerService, IUserService userService, IReportSettingService reportsettingService)
        {
            this.loggerService = loggerService;
            this.userService = userService;
            this.reportsettingService = reportsettingService;
        }

        [HttpGet]
        [AllDepartmentAuthorize]
        public ActionResult Logger()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetLoggers()
        {
            return PartialView("LoggerList", loggerService.GetAllMessages());
        }

    }
}