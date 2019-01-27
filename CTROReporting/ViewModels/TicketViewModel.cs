using CTROLibrary.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CTROReporting.ViewModels
{
    public class TicketViewModel
    {
    }

    public class TicketGenerateViewModel
    {
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        public string Description { get; set; }

        public bool TicketResult { get; set; }

        [Required]
        [Display(Name = "Ticket Option")]
        public string SelectedTicket { get; set; }

        public string TicketName { get; set; }

        public IEnumerable<SelectListItem> Tickets { get; set; }

        public TicketGenerateViewModel()
        {
            TicketResult = false;
        }
    }

    public class TicketLoggerViewModel
    {
        public ApplicationUser user { get; set; }

        public DateTime Nowtime { get; set; }

        public StringBuilder Message { get; set; }

        public TicketLoggerViewModel()
        {
            Message = new StringBuilder();
        }

    }
}