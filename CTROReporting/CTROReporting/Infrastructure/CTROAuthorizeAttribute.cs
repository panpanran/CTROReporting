using CTROReporting.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace CTROReporting.Infrastructure
{
    public class CTROAuthorizeAttribute : AuthorizeAttribute
    {
        public virtual ApplicationUser GetUser(string userid)
        {
            var user = CTROLibrary.CTROFunctions.GetDataFromJson<ApplicationUser>("UserService", "GetByUserID", "userid=" + userid);
            return user;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new
            RouteValueDictionary(new { controller = "ApplicationUser", action = "Login" }));
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }

    }

    public class DepartmentAuthorizeAttribute : CTROAuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized)
            {
                // The user is not authenticated
                return false;
            }

            var user = GetUser(httpContext.User.Identity.GetUserId());
            if (user.Department.DepartmentName == "All")
            {
                // Administrator => let him in
                return true;
            }
            else
            {
                // No id was specified => we do not allow access
                return false;
            }
        }

    }

    public class IsActiveAuthorizeAttribute : CTROAuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized)
            {
                // The user is not authenticated
                return false;
            }

            var user = GetUser(httpContext.User.Identity.GetUserId());
            if (user.Activated == true)
            {
                // Administrator => let him in
                return true;
            }
            else
            {
                // No id was specified => we do not allow access
                return false;
            }
        }
    }
}