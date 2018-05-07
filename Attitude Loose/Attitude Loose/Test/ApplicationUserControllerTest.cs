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

        //public Task CreateTurnroundReportAsync(string startDate, string endDate)
        //{
        //    return Task.Run(() => CreateTurnroundReport(startDate, endDate));
        //}
        [Test()]
        public void WeeklyPDAReportTest()
        {
            CTRPReports ctrpreports = new CTRPReports();
            using (var conn = new NpgsqlConnection(CTRPConst.connString))
            {
                conn.Open();
                try
                {

                    ctrpreports.PDAWorkloadBook(conn,"2018-04-01", "2018-04-30");
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }


        [Test()]
        public void EmialTest()
        {
            CTRPFunctions.SendEmail("Turnround Report", "This is a test email. ", "ran.pan@nih.gov", @"C:\Users\panr2\Downloads\DataWarehouse\Turnround Report\Turnround Report_201803 By Ran Code.xlsx");
        }

        [Test()]
        public void GetBusinessDaysTest()
        {
            try
            {
                List<DateTime> dd = CTRPFunctions.GetBusinessDays(Convert.ToDateTime("2018-01-01").Date, Convert.ToDateTime("2018-01-31").Date, CTRPConst.Holidays);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Test()]
        public void CreateTurnroundReportTest()
        {
            CTRPReports reports = new CTRPReports();
            string s1 = "2018-04-01";
            string s2 = "2018-05-01";
            using (var conn = new NpgsqlConnection(CTRPConst.connString))
            {
                conn.Open();

                try
                {
                    string savepath = CTRPConst.turnround_savepath + "_" + s1.Replace("-", "") + "-" + s2.Replace("-", "") + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx";
                    string templatepath = CTRPConst.turnround_template_file;
                    DataSet conclusionturnroundDS = new DataSet();
                    DataSet turnroundDS = reports.TurnroundBook(conn, s1, s2, out conclusionturnroundDS);

                    CTRPFunctions.WriteExcelByDataSet(turnroundDS, savepath, templatepath, 2, 1);
                    CTRPFunctions.WriteExcelByDataSet(conclusionturnroundDS, savepath, null, 2, 18);

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        [Test()]
        public void CreateSponsorNotMatchReportTest()
        {
            CTRPReports reports = new CTRPReports();
            using (var conn = new NpgsqlConnection(CTRPConst.connString))
            {
                conn.Open();
                try
                {
                    string savepath = CTRPConst.sponsornotmatch_savepath + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx";
                    string templatepath = CTRPConst.sponsornotmatch_template_file;
                    DataSet sponsorDS = reports.SponsorNotMatchBook(conn);

                    CTRPFunctions.WriteExcelByDataSet(sponsorDS, savepath, templatepath, 2, 1);
                    CTRPFunctions.SendEmail("Sponsor Report", "Attached please find. \r\n This is sponsor report generated at " + DateTime.Now.ToString(), "kirsten.larco@nih.gov", savepath);
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
                //FirstName = "ran",
                //LastName = "pan",
                UserName = "panpanr",
                RoleId = 0,
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                DateCreated = DateTime.Now,
                LastLoginTime = DateTime.Now,
                //ProfilePicUrl = null,
            };
            return applicationUser;
        }
    }

}
