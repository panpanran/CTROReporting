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
    public interface IMetricService
    {
        IEnumerable<Metric> GetMetrics();
        IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<Metric> metrics, int selectedId);
        //Metric GetMetric(int id);
        //void CreateMetric(Metric metric);
    }

    public class MetricService : IMetricService
    {
        private readonly IMetricRepository metricRepository;
        private readonly IUnitOfWork unitOfWork;

        public MetricService(IMetricRepository metricRepository, IUnitOfWork unitOfWork)
        {
            this.metricRepository = metricRepository;
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<Metric> GetMetrics()
        {
            var metric = metricRepository.GetAll();
            return metric;
        }

        public IEnumerable<SelectListItem> ToSelectListItems(IEnumerable<Metric> metrics, int selectedId)
        {
            return

                metrics.OrderBy(metric => metric.Type)
                      .Select(metric =>
                          new SelectListItem
                          {
                              Selected = (metric.MetricId == selectedId),
                              Text = metric.Type,
                              Value = metric.MetricId.ToString()
                          });
        }
    }
}