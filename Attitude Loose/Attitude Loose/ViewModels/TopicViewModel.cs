using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Attitude_Loose.ViewModels
{
    public class TopicFormModel
    {
        public TopicFormModel()
        {
            CreatedDate = DateTime.Now;
        }
        public int TopicId { get; set; }

        [Required]
        [StringLength(50)]
        public string TopicName { set; get; }

        [Required(ErrorMessage = "*")]
        [StringLength(100)]
        public string Desc { get; set; }

        [Required(ErrorMessage = "*")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDate { get; set; }

        public bool TopicType { get; set; }

        public string UserId { get; set; }

        public int? MetricId { get; set; }

        public IEnumerable<SelectListItem> Metrics { get; set; }
    }

    public class TopicListViewModel
    {
        public int TopicId { get; set; }

        public string TopicName { set; get; }

        public string Desc { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string CreatedDate { get; set; }

        public int SupportsCount { get; set; }
    }

    public class TopicsPageViewModel
    {
        public IEnumerable<TopicListViewModel> TopicList { get; set; }

        public IEnumerable<SelectListItem> FilterBy { get; set; }

        public IEnumerable<SelectListItem> SortBy { get; set; }

        public TopicsPageViewModel(string selectedFilter, string selectedSort)
        {
            FilterBy = new SelectList(new[]{
                       new SelectListItem{ Text="All", Value="All"},
                       new SelectListItem{ Text="My Topics", Value="My Topics"},
                       new SelectListItem{ Text="My Followed Topics", Value="My Followed Topics"},
                       new SelectListItem{ Text="My Followings Topics", Value="My Followings Topics"}
                       }, "Text", "Value", selectedFilter);
            SortBy = new SelectList(new[]{
                       new SelectListItem{ Text="Date", Value="Date"},
                       new SelectListItem{ Text="Popularity", Value="Popularity"}}, "Text", "Value", selectedSort);

        }
    }
}