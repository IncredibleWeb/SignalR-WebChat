using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Web.Http;

namespace SignalR_Web_Chat_Application.Controllers
{
    public class NotificationHubController : System.Web.Mvc.Controller
    {
        public string Index()
        {
            var notificationHub = System.Web.HttpContext.Current.Application["NotificationHub"] as List<string>;
            if (notificationHub == null)
            {
                notificationHub = new List<string>();
            }

            return JsonConvert.SerializeObject(notificationHub.ToArray());
        }

        public string Add(string value)
        {
            try
            {
                var notificationHub = System.Web.HttpContext.Current.Application["NotificationHub"] as List<string>;
                if (notificationHub == null)
                {
                    notificationHub = new List<string>();
                }

                if (!notificationHub.Contains(value))
                {
                    notificationHub.Add(value);
                }

                System.Web.HttpContext.Current.Application["NotificationHub"] = notificationHub;
                return JsonConvert.SerializeObject(value);
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(false);
            }
        }
    }
}