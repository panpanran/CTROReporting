using Attitude_Loose.Controllers;
using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using Attitude_Loose.Repository;
using Attitude_Loose.Service;
using Attitude_Loose.Test;
using Attitude_Loose.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Moq;
using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace Attitude_Loose.Test
{
    [TestFixture()]
    public class ApplicationUserControllerTest
    {
        Mock<IUserRepository> userRepository;
        Mock<IUserProfileRepository> userProfileRepository;
        Mock<IUnitOfWork> unitOfWork;
        Mock<ControllerContext> controllerContext;
        IUserService userService;
        IUserProfileService userProfileService;
        Mock<ApplicationUserController> applicationuserController;


        [SetUp]
        public void SetUp()
        {
            userRepository = new Mock<IUserRepository>();
            userProfileRepository = new Mock<IUserProfileRepository>();
            unitOfWork = new Mock<IUnitOfWork>();

            userService = new UserService(userRepository.Object, userProfileRepository.Object, unitOfWork.Object);
            userProfileService = new UserProfileService(userProfileRepository.Object, unitOfWork.Object);

            controllerContext = new Mock<ControllerContext>();
            applicationuserController = new Mock<ApplicationUserController>();

        }

        [Test()]
        public void TurnRoundReport()
        {
            var connString = "Server=localhost;Port=5434;User Id=dwprod;Password=dwprod_at_ctrp17;Database=dw_ctrpn";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                // Retrieve all rows
                string path = @"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Code\test.txt";
                string querytext = System.IO.File.ReadAllText(path).Replace("startDate","2018-04-01").Replace("endDate","2018-04-13");

                var cmd = new NpgsqlCommand(querytext, conn);
                NpgsqlDataReader datareader = cmd.ExecuteReader();
                DataTable inputDT = new DataTable();
                inputDT.Load(datareader);

                DateTime tsrdate = new DateTime();
                DateTime receiveddate = new DateTime();
                DateTime onholddate = new DateTime();
                DateTime offholddate = new DateTime();
                DataTable outputDT = inputDT.Clone();
                outputDT.Columns.Add("overalldurations", typeof(Int32));
                outputDT.Columns.Add("onholdtime", typeof(Int32));
                outputDT.Columns.Add("processingtime", typeof(Int32));
                int overalldurations = 0;
                int onholdtime = -100;
                int processingtime = 0;
                try
                {
                    foreach (DataRow row in inputDT.Rows)
                    {
                        tsrdate = (DateTime)row["tsrdate"];
                        receiveddate = (DateTime)row["receiveddate"];
                        onholddate = string.IsNullOrEmpty(row["onholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["onholddate"]);
                        offholddate = string.IsNullOrEmpty(row["offholddate"].ToString()) ? new DateTime(2020, 1, 1) : (DateTime)(row["offholddate"]);
                        if (tsrdate >= receiveddate)
                            overalldurations = CTRPFunctions.CountBusinessDays(receiveddate, tsrdate, CTRPFunctions.Holidays);
                        if (!string.IsNullOrEmpty(row["offholddate"].ToString()) && !string.IsNullOrEmpty(row["offholddate"].ToString()))
                        {
                            if (offholddate >= onholddate)
                            {
                                onholdtime = CTRPFunctions.CountBusinessDays(onholddate, offholddate, CTRPFunctions.Holidays);
                            }
                        }
                        else
                        {
                            onholdtime = 0;
                        }
                        processingtime = overalldurations - onholdtime;
                        if (processingtime > 7)
                        {
                            if (!row.Table.Columns.Contains("overalldurations"))
                            {
                                row.Table.Columns.Add("overalldurations", typeof(Int32));
                                row.Table.Columns.Add("onholdtime", typeof(Int32));
                                row.Table.Columns.Add("processingtime", typeof(Int32));
                            }
                            row["overalldurations"] = overalldurations;
                            row["onholdtime"] = onholdtime;
                            row["processingtime"] = processingtime;

                            outputDT.ImportRow(row);
                        }
                        //CTRPFunctions.SendEmail("dd", "dd", "ran.pan@nih.gov");
                    }

                    CTRPFunctions.CreateExcelByDataTable(outputDT);

                    //var results = inputDT.AsEnumerable().Where(p => (p.Field<DateTime>("tsrdate") - p.Field<DateTime>("receiveddate")).Days -
                    //CTRPFunctions.CountBusinessDays(p.Field<DateTime>("receiveddate"), p.Field<DateTime>("tsrdate"), Holidays) > 8
                    //&& p.Field<DateTime>("receiveddate") > new DateTime(2018, 3, 1));
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        [Test()]
        public void LoginTest()
        {
            var userManager = new UserManager<ApplicationUser>(new TestUserStore());
            ApplicationUserController controller = new ApplicationUserController(userProfileService, userService, userManager);
            var mockAuthenticationManager = new Mock<IAuthenticationManager>();
            mockAuthenticationManager.Setup(am => am.SignOut());
            mockAuthenticationManager.Setup(am => am.SignIn());
            controller.AuthenticationManager = mockAuthenticationManager.Object;
            ApplicationUser applicationUser = getApplicationUser();
            userManager.CreateAsync(applicationUser, "123456");
            var result = controller.Login(new LoginViewModel { UserName = "panpanr", Password = "123456", RememberMe = false }, "abcd").Result;
            Assert.IsNotNull(result);
            var addedUser = userManager.FindByName("panpanr");
            Assert.IsNotNull(addedUser);
            Assert.AreEqual("panpanr", addedUser.UserName);
        }

        [Test()]
        public void RegisterTest()
        {
            var userManager = new UserManager<ApplicationUser>(new TestUserStore());
            ApplicationUserController controller = new ApplicationUserController(userProfileService, userService, userManager);
            var mockAuthenticationManager = new Mock<IAuthenticationManager>();
            mockAuthenticationManager.Setup(am => am.SignOut());
            mockAuthenticationManager.Setup(am => am.SignIn());
            controller.AuthenticationManager = mockAuthenticationManager.Object;
            var result =
                controller.Register(new RegisterViewModel
                {
                    UserName = "chenfei",
                    Email = "chenfei@gmail.com",
                    Password = "123456",
                    ConfirmPassword = "123456"
                }).Result;
            Assert.IsNotNull(result);
            var addedUser = userManager.FindByName("chenfei");
            Assert.IsNotNull(addedUser);
            Assert.AreEqual("chenfei", addedUser.UserName);
        }

        public ApplicationUser getApplicationUser()
        {
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Activated = true,
                Email = "panpanr@gmail.com",
                FirstName = "ran",
                LastName = "pan",
                UserName = "panpanr",
                RoleId = 0,
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                DateCreated = DateTime.Now,
                LastLoginTime = DateTime.Now,
                ProfilePicUrl = null,
            };
            return applicationUser;
        }
    }

}
