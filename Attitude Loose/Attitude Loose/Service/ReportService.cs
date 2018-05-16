using Attitude_Loose.Infrastructure;
using Attitude_Loose.Models;
using Attitude_Loose.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Attitude_Loose.Service
{
    public interface IReportService
    {
        IEnumerable<Report> GetReports();
        IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<Report> reports, string reporttype, int selectedId);
    }

    public class ReportService : IReportService
    {
        private readonly IReportRepository reportRepository;
        private readonly IUnitOfWork unitOfWork;

        public ReportService(IReportRepository reportRepository, IUnitOfWork unitOfWork)
        {
            this.reportRepository = reportRepository;
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<Report> GetReports()
        {
            var report = reportRepository.GetAll();
            return report;
        }

        public IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<Report> reports, string reporttype, int selectedId)
        {
            return
                reports.Where(x=>x.ReportType == reporttype).OrderBy(report => report.ReportId)
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