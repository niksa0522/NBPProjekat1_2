using Microsoft.AspNetCore.SignalR;

namespace NBP_1_2.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotificationAll(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage",user, message);
        }
        public async Task SendNotificationToUser(string user, string message)
        {
            await Clients.User(user).SendAsync("ReceiveMessage", message);
        }
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            //await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            //await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has left the group {groupName}.");
        }
        public async Task MessageGroup(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }
    }
}
