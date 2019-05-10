using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CTROReporting.ViewModels
{
    public class LoggerViewModel
    {
    }

    public class LoggerListViewModel
    {
        public int LogId { get; set; }

        public string Message { get; set; }

        public string UserName { get; set; }

        public DateTime CreatedDate { get; set; }
    }

}