using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using Attitude_Loose.Repository;
using Attitude_Loose.Service;
using Attitude_Loose.Test;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Attitude_Loose.App_Start
{
    public class IOCConfig
    {
        public static void Run()
        {
            SetAutofacContainer();
        }
        private static void SetAutofacContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<DatabaseFactory>().As<IDatabaseFactory>().InstancePerRequest();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();

            builder.RegisterAssemblyTypes(typeof(UserProfileRepository).Assembly)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces();
            //Register ApplicationUserController
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            //Register UserProfileService
            builder.RegisterAssemblyTypes(typeof(UserProfileService).Assembly)
            .Where(t => t.Name.EndsWith("Service"))
            .AsImplementedInterfaces();
            //builder.RegisterType<CTRPSchedule>().InstancePerRequest();

            builder.Register(c => new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new EntitiesInitial())))
                .As<UserManager<ApplicationUser>>().InstancePerRequest();

            var container = builder.Build();
            //Create relationship (Only can be used in MVC)
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}