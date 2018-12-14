using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using CTROLibrary.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CTROReporting.Service
{
    public interface ILoggerService
    {
        void CreateLogger(Logger logger);
        void SaveLogger();
        IEnumerable<Logger> GetLoggersByUser(string userid);
    }

    public class LoggerServiceController : ApiController, ILoggerService
    {
        private readonly ILoggerRepository loggerRepository;
        private readonly IUnitOfWork unitOfWork;

        public LoggerServiceController(ILoggerRepository loggerRepository, IUnitOfWork unitOfWork)
        {
            this.loggerRepository = loggerRepository;
            this.unitOfWork = unitOfWork;
        }

        public void SaveLogger()
        {
            unitOfWork.Commit();
        }

        public void CreateLogger(Logger logger)
        {
            loggerRepository.Add(logger);
            SaveLogger();
        }

        public IEnumerable<Logger> GetLoggersToday()
        {
            var loggers = loggerRepository.GetAll().Where(x => x.CreatedDate.Date == DateTime.Today.Date).OrderByDescending(x => x.CreatedDate);
            return loggers;
        }

        public IEnumerable<Logger> GetLoggersByUser(string userid)
        {
            var loggers = loggerRepository.GetMany(x => x.UserId == userid).OrderByDescending(g => g.CreatedDate);
            return loggers;
        }
    }

}