using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Web.Http;
using System.Net;
using System.Text;
using System.IO;

namespace SignalR_Web_Chat_Application.Controllers
{
    public class NotificationHubController : System.Web.Mvc.Controller
    {
        public string Add(string value, string channel)
        {
            try
            {
                var notificationHub = System.Web.HttpContext.Current.Application[string.Format("{0}{1}", "NotificationHub", channel)] as List<string>;
                if (notificationHub == null)
                {
                    notificationHub = new List<string>();
                }

                if (!notificationHub.Contains(value))
                {
                    notificationHub.Add(value);
                }

                System.Web.HttpContext.Current.Application[string.Format("{0}{1}", "NotificationHub", channel)] = notificationHub;
                return JsonConvert.SerializeObject(value);
            }
            catch (Exception)
            {
                return JsonConvert.SerializeObject(false);
            }
        }


        /// <summary>
        /// Send Push notifications through GCM
        /// 
        /// $.ajax({
        ///     type: "POST",
        ///     url: "https://android.googleapis.com/gcm/send",
        ///     contentType: 'application/json',
        ///     beforeSend: function(request) {
        ///         request.setRequestHeader("Authorization", "key=AIzaSyAF5MPpOxHAeaFJDgzoFg6TdjNiQuiaNoY");
        ///     },
        ///     data: "{\"registration_ids\":[\"" + data + "\"]}"
        /// });
        /// </summary>
        public string Send(string channel, string senderId = "")
        {
            // retrieve the registrations
            var notificationHub = System.Web.HttpContext.Current.Application[string.Format("{0}{1}", "NotificationHub", channel)] as List<string>;
            if (notificationHub == null)
            {
                notificationHub = new List<string>();
            }

            var registrationIds = "";

            // iterate through the notification hub channel to create the recipient list
            foreach (var item in notificationHub)
            {
                if (item != senderId)
                {
                    registrationIds += item + ",";
                }
            }

            if (registrationIds.Length > 0)
            {
                // remove trailing comma
                registrationIds.Trim(',');

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://android.googleapis.com/gcm/send");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "key=AIzaSyAF5MPpOxHAeaFJDgzoFg6TdjNiQuiaNoY");

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"registration_ids\":[\"" + registrationIds + "\"]}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }

                return JsonConvert.SerializeObject(true); ;
            }

            return JsonConvert.SerializeObject(false); ;
        }
    }
}