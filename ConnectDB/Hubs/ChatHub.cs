using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ConnectDB.Hubs
{
    public class ChatHub : Hub
    {
        // When a user connects, they can join a group (e.g., specific to their CustomerId)
        public async Task JoinUserGroup(string customerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{customerId}");
        }

        // Admin joins a special group
        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        // Send message from User to Admin
        public async Task SendMessageToAdmin(string customerId, string message)
        {
            // Send to all admins
            await Clients.Group("Admins").SendAsync("ReceiveMessageFromUser", customerId, message);
            // Optionally, also send back to the user to confirm
            await Clients.Group($"User_{customerId}").SendAsync("ReceiveMessage", "User", message);
        }

        // Send message from Admin to a specific User
        public async Task SendMessageToUser(string customerId, string message)
        {
            // Send to that specific user
            await Clients.Group($"User_{customerId}").SendAsync("ReceiveMessage", "Admin", message);
            // Also send to admins so other admins see the reply
            await Clients.Group("Admins").SendAsync("ReceiveMessageFromAdmin", customerId, message);
        }
    }
}
