using Microsoft.AspNetCore.SignalR;

namespace CenIT.DegreeManagement.CoreAPI.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotificationToUnit(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
