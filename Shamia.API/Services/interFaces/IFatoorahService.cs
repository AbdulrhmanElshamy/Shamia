namespace Shamia.API.Services.interFaces
{
    public interface IFatoorahService
    {
        public  Task<FatoorahResponse> SendPaymentAsync(PaymentRequest request);
        public  Task<PaymentStatusResponse> GetPaymentStatusAsync(string paymentId);
    }
}
