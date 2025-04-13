using Microsoft.EntityFrameworkCore;
using Shamia.API.Services.interFaces;
using Shamia.DataAccessLayer;
using Shamia.DataAccessLayer.Entities;

namespace Shamia.API.Services
{
    public class NotificationService(ShamiaDbContext dbContext) : INotificationService
    {
        public async Task<Notification> Create(Notification notification)
        {
          await  dbContext.Notifications.AddAsync(notification);

            await dbContext.SaveChangesAsync();

            return notification;
        }

        public async Task<IEnumerable<Notification>> GetAll(int page = 0, int pageSize = 10)
        {
            return await dbContext.Notifications.OrderBy(c => c.CreatedAt).Skip(page * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<bool> MakeRead(List<int> Ids)
        {

            await dbContext
                 .Notifications
         .Where(n => Ids.Contains(n.Id) && !n.IsRead)
         .ExecuteUpdateAsync(setters => setters
             .SetProperty(n => n.IsRead, true)
         );

            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
