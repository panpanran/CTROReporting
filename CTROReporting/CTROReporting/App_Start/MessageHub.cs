using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTROReporting.App_Start
{
    public class MessagesHub : Hub
    {
        private static string conString = ConfigurationManager.ConnectionStrings["CTROReportingEntities"].ToString();

        [HubMethodName("sendMessages")]
        public static void SendMessages()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MessagesHub>();
            context.Clients.All.updateMessages();
        }
    }
}
