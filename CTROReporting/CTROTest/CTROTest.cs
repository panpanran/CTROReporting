using CTROLibrary;
using CTROLibrary.CTRO;
using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using CTROLibrary.Repository;
using CTROReporting.Service;
using Moq;
using Npgsql;
using NUnit.Framework;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CTROTest
{
    [TestFixture()]
    public class CTROTest
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

            //recordService = new RecordServiceController(recordRepository.Object, unitOfWork.Object);
            //reportsettingService = new ReportSettingServiceController(reportsettingRepository.Object, unitOfWork.Object);
            //reportService = new ReportServiceController(reportRepository.Object, unitOfWork.Object, recordService, reportsettingService);            
            reportService = new Mock<IReportService>();
        }


        [Test()]
        public void WebApiTest()
        {
            string url = "http://local.ctroreporting.com";
            string username = "panpanr2";
            string password = "panpanr2an";
            string result = CTROFunctions.GetToken(url, username, password);
            var schedules = CTROFunctions.GetDataFromJson<List<Report>>("ReportService", "GetReports");
        }

        [Test()]
        public void CTROConstTest()
        {
            CTROConst st = new CTROConst();
            string a = CTROConst.EW_ACCOUNT;
            string b = CTROConst.EW_PASSWORD;
            string c = CTROConst.PA_ACCOUNT;
            string d = CTROConst.PA_PASSWORD;
        }


        [Test()]
        public async Task ScheduleTest()
        {
            try
            {
                var schedules = CTROFunctions.GetDataFromJson<List<Schedule>>("ScheduleService", "GetSchedules");
                var report = CTROFunctions.GetDataFromJson<Report>("ReportService", "GetReportById", "reportid=3");
                var user = CTROFunctions.GetDataFromJson<ApplicationUser>("UserService", "GetByUserID", "userid=1551f213-a46f-4673-a6b9-64d683b5048b");
                string startdate = schedules[0].StartTime.AddDays(-7).ToString("yyyy-MM-dd");
                string enddate = schedules[0].StartTime.ToString("yyyy-MM-dd");

                Record record = new Record
                {
                    ReportId = 3,
                    UserId = "1551f213-a46f-4673-a6b9-64d683b5048b",
                    StartDate = startdate,
                    EndDate = enddate
                };

                var url = await CTROFunctions.CreateDataFromJson<Record>("RecordService", "CreateRecord", record);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }


        [Test()]
        public void ScheduleServiceTest()
        {
            try
            {
                if (StdSchedulerFactory.GetDefaultScheduler().Result.IsStarted)
                {
                    StdSchedulerFactory.GetDefaultScheduler().Result.Shutdown();
                }
                string json = new WebClient().DownloadString("http://local.CTROLibrary.com/api/ScheduleService/GetSchedules");
                List<Schedule> schedulelist = new JavaScriptSerializer().Deserialize<List<Schedule>>(json);
                CTROSchedule ctroschedule = new CTROSchedule();
                ctroschedule.Start(schedulelist);
            }
            catch (Exception ex)
            {
                Logging.WriteLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
        }

        [Test()]
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
            Type type = Type.GetType("CTROLibrary.CTRO." + "PDAAbstraction" + "Report");
            Object obj = Activator.CreateInstance(type);
            MethodInfo methodInfo = type.GetMethod("CreateBook");
            object classInstance = Activator.CreateInstance(type, null);
            DataSet bookDS = null;
            string[] variables = { "worknumber", "worktime" };
            List<Dictionary<string, string>> Yaxis = new List<Dictionary<string, string>>();
            Dictionary<string, string> tempYaxis = new Dictionary<string, string>();
            Dictionary<string, string> temprankYaxis = new Dictionary<string, string>();
            using (var conn = new NpgsqlConnection(CTROConst.connString))
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
            Type type = Type.GetType("CTROLibrary.CTRO.SponsorReport");
            Object obj = Activator.CreateInstance(type);
            MethodInfo methodInfo = type.GetMethod("CreateBook");
            object classInstance = Activator.CreateInstance(type, null);
            object bookDS = null;
            using (var conn = new NpgsqlConnection(CTROConst.connString))
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
                    CTROFunctions.WriteExcelByDataSet((DataSet)bookDS, savepath, templatepath, 2, 1);
                    if (conclusionDS.Tables.Count > 0)
                    {
                        CTROFunctions.WriteExcelByDataSet(conclusionDS, savepath, null, 2, 18);
                    }
                    CTROFunctions.SendEmail("Turnround Report", "Attached please find. \r\n This is turnround report from " + "2018-05-01" + " to " + "2018-05-02", "ran.pan@nih.gov", savepath);
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
            CTROFunctions.SendEmail("Turnround Report", "This is a test email. ", "ran.pan@nih.gov", string.Empty);
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
                //DateTime lastworkdate = Convert.ToDateTime("2019-02-20");
                //while (CTROFunctions.CountBusinessDays(lastworkdate, Convert.ToDateTime("2019-01-22"), CTROConst.Holidays) <= 1)
                //{
                //    lastworkdate = lastworkdate.AddDays(-1);
                //}
                List<DateTime> dd = CTROFunctions.GetBusinessDays(Convert.ToDateTime("2018-03-26").Date, Convert.ToDateTime("2018-09-25").Date, CTROConst.Holidays);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Test()]
        public void ConnectNpgSQL()
        {
            using (var conn = new NpgsqlConnection("Server=localhost;Port=5434;User Id=panran;Password=Prss_1234;Database=dw_ctrpn"))
            {
                conn.Open();

                try
                {
                    string ran = "";
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
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
        //            string savepath = CTRPConst.biomarker_savepath + "_" + String.Format("{0:yyyymmddhhmmss}", DateTime.Now) + ".xlsx";
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

        //            CTROFunctions.WriteExcelByDataSet((DataSet)outputDS, savepath, templatepath, 2, 1);

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
        //    var result = controller.Login(new LoginViewModel { UserName = "panpanr2", Password = "123456", RememberMe = false }, "abcd").Result;
        //    Assert.IsNotNull(result);
        //    var addedUser = userManager.FindByName("panpanr2");
        //    Assert.IsNotNull(addedUser);
        //    Assert.AreEqual("panpanr2", addedUser.UserName);
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
                Email = "panpanr2@gmail.com",
                //FirstName = "ran",
                //LastName = "pan",
                UserName = "panpanr2",
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
