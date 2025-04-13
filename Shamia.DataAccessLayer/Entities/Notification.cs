namespace Shamia.DataAccessLayer.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}