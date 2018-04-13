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
                string querytext = "SELECT distinct s.nci_id \"trialid\"," +
  "s.category \"trialtype\"," +
  "s.submission_number \"submission\"," +
  "s.summary_4_funding_category \"summary4fundingcategory\"," +
  "s.lead_org AS \"leadorganization\"," +
  "r.date_last_updated AS \"receiveddate\"," +
  "tsr.date_last_updated AS \"tsrdate\"," +
  "h.on_hold_date AS \"onholddate\"," +
  "h.off_hold_date AS \"offholddate\"," +
  "h.reason AS \"onholdreason\"," +
  "h.reason_description AS \"onholddescription\"," +
  "'' \"additionalcomments\"," +
  "s.processing_status AS \"processingstatus\"" +
"FROM dw_study s " +
"JOIN dw_study_milestone tsr ON tsr.nci_id = s.nci_id " +
"JOIN dw_study_milestone r ON r.nci_id = s.nci_id " +
"LEFT JOIN dw_study_on_hold_status h ON s.nci_id = h.nci_id " +
"WHERE  s.processing_status != 'Rejected' " +
"AND s.processing_status != 'Submission Terminated' " +
"AND s.category = 'Complete' " +
"AND tsr.submission_number > 1 " +
"AND tsr.date_last_updated > '2018-04-01'" +
"AND tsr.submission_number = s.submission_number " +
"AND tsr.name = \'Ready for Trial Summary Report Date\' " +
"AND r.name = \'Submission Received Date\' " +
"AND r.submission_number = s.submission_number " +
"AND tsr.submission_number = s.submission_number " +
"ORDER BY tsr.date_last_updated; ";

                var cmd = new NpgsqlCommand(querytext, conn);
                NpgsqlDataReader datareader = cmd.ExecuteReader();
                DataTable inputDT = new DataTable();
                inputDT.Load(datareader);
                DateTime[] Holidays = new DateTime[]{
                    new DateTime(2018,1,1),
                    new DateTime(2018,1,15),
                    new DateTime(2018,2,19),
                    new DateTime(2018,5,28),
                    new DateTime(2018,7,4),
                    new DateTime(2018,9,3),
                    new DateTime(2018,9,3),
                    new DateTime(2018,10,8),
                    new DateTime(2018,11,12),
                    new DateTime(2018,11,22),
                    new DateTime(2018,12,25)
                };
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
                            overalldurations = CTRPFunctions.CountBusinessDays(receiveddate, tsrdate, Holidays);
                        if (!string.IsNullOrEmpty(row["offholddate"].ToString()) && !string.IsNullOrEmpty(row["offholddate"].ToString()))
                        {
                            if (offholddate >= onholddate)
                            {
                                onholdtime = CTRPFunctions.CountBusinessDays(onholddate, offholddate, Holidays);
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
