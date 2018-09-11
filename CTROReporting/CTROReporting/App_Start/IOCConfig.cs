using CTRPReporting.Infrastructure;
using CTRPReporting.Models;
using CTRPReporting.Repository;
using CTRPReporting.Service;
using CTRPReporting.Test;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CTRPReporting.App_Start
{
    public class IOCConfig
    {
        public static void Run()
        {
            SetAutofacContainer();
            //SetApiContainer();
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
            builder.RegisterAssemblyTypes(typeof(IUserProfileService).Assembly)
            .Where(t => t.Name.EndsWith("Service"))
            .AsImplementedInterfaces();
            //Register UserProfileService
            builder.RegisterAssemblyTypes(typeof(UserProfileServiceController).Assembly)
            .Where(t => t.Name.EndsWith("ServiceController"))
            .AsImplementedInterfaces();
            //builder.RegisterType<CTRPSchedule>().InstancePerRequest();

            builder.Register(c => new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new EntitiesInitial())))
                .As<UserManager<ApplicationUser>>().InstancePerRequest();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            // OPTIONAL: Register the Autofac model binder provider.
            builder.RegisterWebApiModelBinderProvider();



            var container = builder.Build();
            //Create relationship (Only can be used in MVC)
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static void SetApiContainer()
        {
            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            // OPTIONAL: Register the Autofac model binder provider.
            builder.RegisterWebApiModelBinderProvider();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}