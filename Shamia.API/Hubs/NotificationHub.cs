using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shamia.DataAccessLayer;

namespace Shamia.API.Hubs
{
    public class NotificationHub(ShamiaDbContext dbContext) : Hub
    {
        public async Task SendMessage(string message)
        {
            var userIds = await dbContext.Users.Where(c => c.Role == "Admin").Select(c => c.Id).ToListAsync();

            await Clients.Users(userIds).SendAsync("ReceiveMessage", message);
        }

    }
}
