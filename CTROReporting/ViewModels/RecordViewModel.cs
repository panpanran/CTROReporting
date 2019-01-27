using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTROReporting.ViewModels
{
    public class RecordViewModel
    {
    }

    public class RecordListViewModel
    {
        public int RecordId { get; set; }

        public string ReportName { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string UserName { get; set; }

        public string FilePath { get; set; }

        public string CreatedDate { get; set; }
    }

    public class RecordsPageViewModel
    {
        public IEnumerable<RecordListViewModel>RecordList { get; set; }
    }

}