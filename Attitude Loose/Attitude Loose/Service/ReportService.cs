using Attitude_Loose.CTRO;
using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using Attitude_Loose.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Attitude_Loose.Service
{
    public interface IReportService
    {
        IEnumerable<Report> GetReports();
        Report GetReportById(int reportid);
        IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<Report> reports, string reporttype, int selectedId);
        Report GetByReportName(string name);
        bool CreateReport(int selectedreport, string userid, string startdate, string enddate, string email);
    }

    public class ReportService : IReportService
    {
        private readonly IReportRepository reportRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IRecordService recordService;

        public ReportService(IReportRepository reportRepository, IUnitOfWork unitOfWork, IRecordService recordService)
        {
            this.recordService = recordService;
            this.reportRepository = reportRepository;
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<Report> GetReports()
        {
            var report = reportRepository.GetAll();
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

        public bool CreateReport(int selectedreport, string userid, string startdate, string enddate, string email)
        {
            bool result = false;
            Record record = new Record
            {
                ReportId = selectedreport,
                UserId = userid,
                StartDate = startdate,
                EndDate = enddate,
            };

            string reportname = GetReportById(selectedreport).ReportName.Replace(" - ", "");
            CTROHome home = new CTROHome();
            string savepath = "";
            int report = home.CreateReport(startdate, enddate, email, reportname, out savepath);

            if (report == 1)
            {
                record.FilePath = "../Excel/" + Path.GetFileName(savepath);
                recordService.CreateRecord(record);
                result = true;
            }

            return result;
        }

        public IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<Report> reports, string reporttype, int selectedId)
        {
            return
                reports.Where(x => x.ReportType == reporttype).OrderBy(report => report.ReportId)
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