using CTRPReporting.Controllers;
using CTRPReporting.CTRO;
using CTRPReporting.Infrastructure;
using CTRPReporting.Models;
using CTRPReporting.Repository;
using CTRPReporting.Service;
using CTRPReporting.Test;
using CTRPReporting.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Moq;
using MySql.Data.MySqlClient;
using Npgsql;
using NUnit.Framework;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace CTRPReporting.Test
{

    [TestFixture()]
    public class CTRPTest
    {
        Mock<IReportRepository> reportRepository;
        Mock<IRecordRepository> recordRepository;
        Mock<IUnitOfWork> unitOfWork;
        Mock<IReportSettingRepository> reportsettingRepository;

        IRecordService recordService;
        Mock<IReportService> reportService;
        IReportSettingService reportsettingService;


        [SetUp]
        public void SetUp()
        {
            reportRepository = new Mock<IReportRepository>();
            recordRepository = new Mock<IRecordRepository>();
            reportsettingRepository = new Mock<IReportSettingRepository>();
            unitOfWork = new Mock<IUnitOfWork>();

            recordService = new RecordServiceController(recordRepository.Object, unitOfWork.Object);
            reportsettingService = new ReportSettingServiceController(reportsettingRepository.Object, unitOfWork.Object);
            //reportService = new ReportServiceController(reportRepository.Object, unitOfWork.Object, recordService, reportsettingService);            
            reportService = new Mock<IReportService>(); ;
        }

        [Test]
        public void GetReportsTest()
        {
            var itemMock = new Mock<Report>();
            var items = new List<Report> { itemMock.Object }; //<--IEnumerable<IMyObject>
            //reportRepository.Setup(x=>x.GetAll()).Returns()
            var reportlist = reportService.Setup(x => x.GetReports()).Returns(items);
            var reportresult = reportService.Object.GetReports();
            Assert.AreEqual(reportresult, items, "not matching");
        }

        [Test()]
        public void CreateAnalysisChart()
        {
            Type type = Type.GetType("CTRPReporting.Test." + "PDAAbstraction" + "Report");
            Object obj = Activator.CreateInstance(type);
            MethodInfo methodInfo = type.GetMethod("CreateBook");
            object classInstance = Activator.CreateInstance(type, null);
            DataSet bookDS = null;
            string[] variables = { "worknumber", "worktime" };
            List<Dictionary<string, string>> Yaxis = new List<Dictionary<string, string>>();
            Dictionary<string, string> tempYaxis = new Dictionary<string, string>();
            Dictionary<string, string> temprankYaxis = new Dictionary<string, string>();
            using (var conn = new NpgsqlConnection(CTRPConst.connString))
            {
                conn.Open();
                try
                {
                    //DataTable tempDT = reports.CreateBook(conn, startDate, endDate, out rankDT);
                    string savepath = "";
                    string templatepath = "";
                    DataSet rankDS = new DataSet();
                    object[] parametersArray = new object[] { conn, "2018-05-01", "2018-05-02", "", "", rankDS };
                    bookDS = (DataSet)methodInfo.Invoke(classInstance, parametersArray);
                    savepath = parametersArray[3].ToString();
                    templatepath = parametersArray[4].ToString();
                    rankDS = (DataSet)parametersArray[5];

                    string[] Loginname = rankDS.Tables[0].AsEnumerable().Select(x => x.Field<string>("loginname")).Distinct().ToArray();
                    List<string> tempdates = bookDS.Tables[0].AsEnumerable().OrderBy(x => x.Field<int>("completeddate")).Select(x => x.Field<int>("completeddate").ToString()).Distinct().ToList();
                    string[] Xaxis = new string[] { string.Join(",", tempdates), "", string.Join(",", tempdates), "" };
                    string[] ChartName = new string[] { "Daily Number Chart", "Work Number Rank Chart", "Daily Efficiency Chart", "Work Efficiency Rank Chart" };
                    string[] ChartType = new string[] { "line", "bar", "line", "bar" };
                    string[] XLabel = new string[] { "Date", "PDA Team", "Date", "PDA Team" };
                    string[] YLabel = new string[] { "Number", "Avg Time Per Work", "Number", "Avg Time Per Work" };

                    foreach (string s in variables)
                    {
                        foreach (string ln in Loginname)
                        {
                            string worktotal = rankDS.Tables[0].AsEnumerable().Where(x => x.Field<string>("loginname") == ln).Select(x => x.Field<string>(s)).FirstOrDefault();
                            List<string> Yvalue = new List<string>();
                            foreach (string tdate in tempdates)
                            {
                                string tempvalue = bookDS.Tables[0].AsEnumerable()
                                    .Where(x => x.Field<string>("loginname") == ln && x.Field<int>("completeddate") == Convert.ToInt32(tdate))
                                    .Select(x => x.Field<string>(s)).FirstOrDefault();

                                if (string.IsNullOrEmpty(tempvalue))
                                {
                                    Yvalue.Add("0");
                                }
                                else
                                {
                                    Yvalue.Add(tempvalue);
                                }
                            }
                            tempYaxis.Add(ln, string.Join(",", Yvalue));


                            if (string.IsNullOrEmpty(worktotal))
                            {
                                worktotal = "0";
                            }

                            temprankYaxis.Add(ln, worktotal);
                        }
                        Yaxis.Add(tempYaxis);
                        temprankYaxis = temprankYaxis.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                        Yaxis.Add(temprankYaxis);
                        tempYaxis = new Dictionary<string, string>();
                        temprankYaxis = new Dictionary<string, string>();
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
        [Test()]
        public void CreateReportAsync()
        {
            Type type = Type.GetType("CTRPReporting.Test.SponsorReport");
            Object obj = Activator.CreateInstance(type);
            MethodInfo methodInfo = type.GetMethod("CreateBook");
            object classInstance = Activator.CreateInstance(type, null);
            object bookDS = null;
            using (var conn = new NpgsqlConnection(CTRPConst.connString))
            {
                conn.Open();
                try
                {
                    string savepath = "";
                    string templatepath = "";
                    DataSet conclusionDS = new DataSet();
                    object[] parametersArray = new object[] { conn, "2018-05-01", "2018-05-02", "", "", conclusionDS };
                    bookDS = methodInfo.Invoke(classInstance, parametersArray);
                    //DataSet bookDS = reports.CreateBook(conn, startDate, endDate, out savepath, out templatepath, out conclusionDS);
                    CTRPFunctions.WriteExcelByDataSet((DataSet)bookDS, savepath, templatepath, 2, 1);
                    if (conclusionDS.Tables.Count > 0)
                    {
                        CTRPFunctions.WriteExcelByDataSet(conclusionDS, savepath, null, 2, 18);
                    }
                    CTRPFunctions.SendEmail("Turnround Report", "Attached please find. \r\n This is turnround report from " + "2018-05-01" + " to " + "2018-05-02", "ran.pan@nih.gov", savepath);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        [Test()]
        public void EmailTest()
        {
            CTRPFunctions.SendEmail("Turnround Report", "This is a test email. ", "ran.pan@nih.gov", string.Empty);
        }
        [Test()]
        public void RelativePath()
        {
            string savepath = Environment.CurrentDirectory;
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
        public void ConnectMySQL()
        {
            //using (var conn = new MySqlConnection(CTRPConst.EWconnString))
            //{
            //    conn.Open();

            //    try
            //    {
            //        string ran = "";
            //    }
            //    catch (Exception ex)
            //    {
            //        throw;
            //    }
            //}
        }

        //[Test()]
        //public void CreateReportTest()
        //{
        //    SDABiomarkerReport reports = new SDABiomarkerReport();
        //    using (var conn = new NpgsqlConnection(CTRPConst.connString))
        //    {
        //        conn.Open();
        //        try
        //        {
        //            DataSet outputDS = new DataSet();
        //            DataSet conclusionDS = new DataSet();
        //            string savepath = CTRPConst.biomarker_savepath + "_" + String.Format("{0:yyyyMMddHHmmss}", DateTime.Now) + ".xlsx";
        //            string templatepath = CTRPConst.biomarker_template_file;
        //            NpgsqlCommand cmd = null;
        //            NpgsqlDataReader datareader = null;
        //            //login_name
        //            string[] status_name = { "PENDING", "ACTIVE", "DELECTED_IN_CADSR" };
        //            foreach (string name in status_name)
        //            {
        //                string pdaabstractortext = System.IO.File.ReadAllText(CTRPConst.biomarker_original_file).Replace("statusVal", name);
        //                cmd = new NpgsqlCommand(pdaabstractortext, conn);
        //                datareader = cmd.ExecuteReader();
        //                DataTable nciDT = new DataTable();
        //                nciDT.Load(datareader);
        //                nciDT.TableName = name;
        //                outputDS.Tables.Add(nciDT);
        //            }

        //            CTRPFunctions.WriteExcelByDataSet((DataSet)outputDS, savepath, templatepath, 2, 1);

        //        }
        //        catch (Exception ex)
        //        {
        //            throw;
        //        }
        //    }
        //}

        //[Test()]
        //public void LoginTest()
        //{
        //    var userManager = new UserManager<ApplicationUser>(new TestUserStore());
        //    ApplicationUserController controller = new ApplicationUserController(userProfileService, userService, userManager);
        //    var mockAuthenticationManager = new Mock<IAuthenticationManager>();
        //    mockAuthenticationManager.Setup(am => am.SignOut());
        //    mockAuthenticationManager.Setup(am => am.SignIn());
        //    controller.AuthenticationManager = mockAuthenticationManager.Object;
        //    ApplicationUser applicationUser = getApplicationUser();
        //    userManager.CreateAsync(applicationUser, "123456");
        //    var result = controller.Login(new LoginViewModel { UserName = "panpanr", Password = "123456", RememberMe = false }, "abcd").Result;
        //    Assert.IsNotNull(result);
        //    var addedUser = userManager.FindByName("panpanr");
        //    Assert.IsNotNull(addedUser);
        //    Assert.AreEqual("panpanr", addedUser.UserName);
        //}

        //[Test()]
        //public void RegisterTest()
        //{
        //    var userManager = new UserManager<ApplicationUser>(new TestUserStore());
        //    ApplicationUserController controller = new ApplicationUserController(userProfileService, userService, userManager);
        //    var mockAuthenticationManager = new Mock<IAuthenticationManager>();
        //    mockAuthenticationManager.Setup(am => am.SignOut());
        //    mockAuthenticationManager.Setup(am => am.SignIn());
        //    controller.AuthenticationManager = mockAuthenticationManager.Object;
        //    var result =
        //        controller.Register(new RegisterViewModel
        //        {
        //            UserName = "chenfei",
        //            Email = "chenfei@gmail.com",
        //            Password = "123456",
        //            ConfirmPassword = "123456"
        //        }).Result;
        //    Assert.IsNotNull(result);
        //    var addedUser = userManager.FindByName("chenfei");
        //    Assert.IsNotNull(addedUser);
        //    Assert.AreEqual("chenfei", addedUser.UserName);
        //}

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
                CreatedDate = DateTime.Now,
                LastLoginTime = DateTime.Now,
                //ProfilePicUrl = null,
            };
            return applicationUser;
        }
    }

}
