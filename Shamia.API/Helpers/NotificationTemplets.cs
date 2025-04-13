namespace Shamia.API.Helpers
{
    public static class NotificationTemplets
    {
        public static string SetMessage(string customerName)
        {
           return  $"🛒 A new order has been placed by {customerName}";
        }
    }
}
