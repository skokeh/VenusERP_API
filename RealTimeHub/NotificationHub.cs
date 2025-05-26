using Microsoft.AspNetCore.SignalR;

namespace VenusERP_API.RealTimeHub
{
    public class NotificationHub : Hub
    {
        public async Task SendZatcaResponse(string message)
        {
            await Clients.All.SendAsync("ZatcaResponse", message);
        }
    }
}
