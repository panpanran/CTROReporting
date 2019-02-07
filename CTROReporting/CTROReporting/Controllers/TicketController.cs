using CTROLibrary.Model;
using CTROLibrary.EW;
using CTROReporting.Service;
using CTROReporting.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using CTROLibrary.CTRO;
using CTROLibrary.Infrastructure;

namespace CTROReporting.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ILoggerService loggerService;
        private readonly IUserService userService;
        private readonly IReportSettingService reportsettingService;

        public TicketController(ILoggerService loggerService, IUserService userService, IReportSettingService reportsettingService)
        {
            this.loggerService = loggerService;
            this.userService = userService;
            this.reportsettingService = reportsettingService;
        }

        [HttpGet]
        [AllDepartmentAuthorize]
        public ActionResult Ticket()
        {
            var model = new TicketGenerateViewModel();
            var subclassTypes = Assembly.GetAssembly(typeof(EWTicket)).GetTypes().Where(t => t.IsSubclassOf(typeof(EWTicket)) && t.Name.Contains("EWSolution"));
            model.Tickets = subclassTypes.Select(c =>
                          new SelectListItem
                          {
                              Selected = false,
                              Text = c.Name,
                              Value = c.Name
                          });
            return View(model);
        }

        [HttpPost]
        public ActionResult Ticket(TicketGenerateViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = userService.GetByUserID(User.Identity.GetUserId());

                switch (model.SelectedTicket)
                {
                    case "EWSolutionTSRFeedback":
                        Type type = Type.GetType("CTROLibrary.EW." + model.SelectedTicket + ", CTROLibrary");
                        MethodInfo methodInfo = type.GetMethod("BulkUpdate");
                        object classInstance = Activator.CreateInstance(type, null);
                        string where = "assigned_to_=%27Ran%20Pan%27%20and category like '%2519%25' and%20modified_by%20not%20like%20%27%25panr2%25%27";
                        object[] parametersArray = new object[] { where, user };
                        model.TicketResult = (bool)methodInfo.Invoke(classInstance, parametersArray);
                        break;
                    case "EWSolutionTicketTriagingSchedule":
                        EWSolutionTicketTriagingSchedule ew = new EWSolutionTicketTriagingSchedule();
                        model.TicketResult = ew.ScheduleTicketsTriaging();
                        break;
                }
            }
            var subclassTypes = Assembly.GetAssembly(typeof(EWTicket)).GetTypes().Where(t => t.IsSubclassOf(typeof(EWTicket)) && t.Name.Contains("EWSolution"));
            model.Tickets = subclassTypes.Select(c =>
              new SelectListItem
              {
                  Selected = false,
                  Text = c.Name,
                  Value = c.Name
              });
            return View(model);
        }

        public PartialViewResult TicketLogging()
        {
            TicketLoggerViewModel model = new TicketLoggerViewModel();
            IEnumerable<Logger> logs = loggerService.GetLoggersByUser(User.Identity.GetUserId()).Where(l => l.CreatedDate.Date == DateTime.Today.Date);
            foreach (Logger log in logs)
            {
                model.Message.Append(log.Message + "<br>");
            }
            return PartialView(model);
        }
    }
}