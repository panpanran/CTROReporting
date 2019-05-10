using CTROLibrary.Infrastructure;
using CTROLibrary.Model;
using CTROLibrary.Repository;
using CTROReporting.App_Start;
using CTROReporting.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace CTROReporting.Service
{
    public interface ILoggerService
    {
        void CreateLogger(Logger logger);
        void SaveLogger();
        IEnumerable<Logger> GetLoggersByUser(string userid);
        IEnumerable<LoggerListViewModel> GetAllMessages();
    }

    [Authorize]
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

        private string _connString = ConfigurationManager.ConnectionStrings["CTROReportingEntities"].ConnectionString;

        public IEnumerable<LoggerListViewModel> GetAllMessages()
        {
            var messages = new List<LoggerListViewModel>();
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"SELECT [LogId], 
                [Message] FROM [dbo].[Loggers]", connection))
                {
                    command.Notification = null;

                    var dependency = new SqlDependency(command);

                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();

                    messages = GetLoggersToday().ToList().Select(x => new LoggerListViewModel
                    {
                        LogId = x.LogId,
                        Message = x.Message,
                        UserName = x.User.UserName,
                        CreatedDate = x.CreatedDate
                    }).ToList();
                }

            }
            return messages;
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                MessagesHub.SendMessages();
            }
        }
    }

}