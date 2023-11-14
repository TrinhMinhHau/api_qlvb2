using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;

namespace CenIT.DegreeManagement.CoreAPI.Processor.SendNotification
{
    public class FirebaseNotificationUtils
    {
        public async Task SendNotificationToSingleDeviceAsync(string notificationTitle, string notificationBody, List<DeviceTokenModel> tokens)
        {
            foreach (var token in tokens)
            {
                var message = new Message()
                {
                    Notification = new Notification
                    {
                        Title = notificationTitle,
                        Body = notificationBody,
                    },
                    Token = token.DeviceToken,
                };

                var messaging = FirebaseMessaging.DefaultInstance;
                var response = await messaging.SendAsync(message).ConfigureAwait(true);
            }
        }
    }
}
