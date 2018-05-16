using Attitude_Loose.Models;
using Attitude_Loose.Service;
using Attitude_Loose.ViewModels;
using AutoMapper;
using BootstrapMvc;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace Attitude_Loose.Controllers
{
    [Authorize]
    public class TopicController : Controller
    {
        private readonly IMetricService metricService;
        private readonly ITopicService topicService;
        public TopicController(IMetricService metricService, ITopicService topicService)
        {
            this.metricService = metricService;
            this.topicService = topicService;
        }

        public PartialViewResult Create()
        {
            var createTopic = new TopicFormModel();
            var metrics = metricService.GetMetrics();
            //var goalstatus = goalStatusService.GetGoalStatus();
            createTopic.Metrics = metricService.ToSelectListItems(metrics, -1);
            return PartialView(createTopic);
        }

        [HttpPost]
        public ActionResult Create(TopicFormModel createTopic)
        {
            Topic topic = Mapper.Map<TopicFormModel, Topic>(createTopic);
            //var errors = topicService.CanAddGoal(goal, updateService).ToList();
            //ModelState.AddModelErrors(errors);
            if (ModelState.IsValid)
            {
                topicService.CreateTopic(topic);
            }
            return RedirectToAction("TopicList", "Topic");
        }

        //public ActionResult Topicslist(int sortby, int filter)
        //{
        //    if (sortby == 0 && filter == 0)
        //    {
        //        var goals = topicService.GetTopics();
        //        return PartialView("_Goalslist", goals);
        //    }
        //    return PartialView();
        //}

        public ActionResult TopicList(string sortBy = "Date", string filterBy = "All", int page = 0)
        {
            var topics = topicService.GetTopicsByPage(User.Identity.GetUserId(), page, 5, sortBy, filterBy).ToList();

            var topicsViewModel = Mapper.Map<IEnumerable<Topic>, IEnumerable<TopicListViewModel>>(topics).ToList();
            var goalsList = new TopicsPageViewModel(filterBy, sortBy);
            goalsList.TopicList = topicsViewModel;

            if (Request.IsAjaxRequest())
            {
                return Json(topicsViewModel, JsonRequestBehavior.AllowGet);
            }
            return View("TopicList", goalsList);
        }

        [HttpGet]
        public ActionResult Test()
        {
            return View();
        }

    }
}
