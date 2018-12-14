using CTROLibrary.CTRO;
using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using CTROLibrary.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace CTROReporting.Service
{
    public interface IReportService
    {
        IEnumerable<Report> GetReports();
        Report GetReportById(int reportid);
        IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<Report> reports, int selectedId);
        Report GetByReportName(string name);
        Task<bool> CreateReport(int selectedreport, string userid, string startdate, string enddate, ApplicationUser user);
    }

    public class ReportServiceController : ApiController,IReportService
    {
        private readonly IReportRepository reportRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IRecordService recordService;
        private readonly IReportSettingService recportsettingService;

        public ReportServiceController(IReportRepository reportRepository, IUnitOfWork unitOfWork, IRecordService recordService, IReportSettingService recportsettingService)
        {
            this.recordService = recordService;
            this.recportsettingService = recportsettingService;
            this.reportRepository = reportRepository;
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<Report> GetReports()
        {
            var report = reportRepository.GetAll().Where(x =>x.Active == true);
            return report;
        }

        public Report GetReportById(int reportid)
        {
            var report = reportRepository.Get(x => x.ReportId == reportid);
            return report;
        }

        public Report GetByReportName(string name)
        {
            var schedule = reportRepository.Get(x => x.ReportName == name);
            return schedule;
        }

        public async Task<bool> CreateReport(int selectedreport, string userid, string startdate, string enddate, ApplicationUser user)
        {
            bool result = false;

            Report report = GetReportById(selectedreport);
            string reportname = report.ReportName.Replace(" - ", "");

            CTROHome home = new CTROHome();
            string savepath = await home.CreateReport(startdate, enddate, user, report);

            if (!string.IsNullOrEmpty(savepath))
            {
                result = true;
            }

            return result;
        }

        public IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<Report> reports, int selectedId)
        {
            return
                reports.OrderBy(report => report.ReportName)
                      .Select(report =>
                          new SelectListItem
                          {
                              Selected = (report.ReportId == selectedId),
                              Text = report.ReportName,
                              Value = report.ReportId.ToString()
                          });
        }
    }
}