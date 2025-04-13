using Shamia.DataAccessLayer.Entities;

namespace Shamia.API.Services.interFaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetAll(int page = 0, int pageSize = 10);
        Task<Notification> Create(Notification notification);

        Task<bool> MakeRead(List<int> Ids);
    }
}
