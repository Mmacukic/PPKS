using Microsoft.AspNetCore.SignalR;

namespace PPKS_projekt.Hubs;

public class OrderHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext?.Request.Query.TryGetValue("shipmentId", out var shipmentId) == true)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"shipment-{shipmentId}");
        }

        await base.OnConnectedAsync();
    }
    
}