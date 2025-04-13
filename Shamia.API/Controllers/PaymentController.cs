using System.Text;
using System.Text.Json.Serialization;
using Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shamia.API.Common;
using Shamia.API.Dtos.Request;
using Shamia.API.Services;
using Shamia.API.Services.interFaces;
using Shamia.API.Services.InterFaces;
using Shamia.DataAccessLayer;
using Shamia.DataAccessLayer.Entities;

namespace Shamia.API.Controllers
{
    public class PaymentController: ControllerBase
    {

        private readonly IFatoorahService _fatoorahService;
        private readonly ShamiaDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<WebhookController> _logger;


        public PaymentController(IFatoorahService fatoorahService, ShamiaDbContext context, IEmailService emailService, ILogger<WebhookController> logger)
        {
            _fatoorahService = fatoorahService;
            _context = context;
            _emailService = emailService;
            _logger = logger;

        }

        //[HttpPost("checkout")]
        //public async Task<IActionResult> Checkout()
        //{
        //    //[FromBody] CheckoutRequest request
        //    // Create and save your order here

        //    var order = new Order
        //    {
        //        OrderDate = DateTime.Now,
        //        Status_En = "Pending",
        //        Status_Ar = "قيد الانتظار",
        //        //UserId = "4d176098-0cda-4100-83ab-123ecb17335f",
               
        //        Total_Amount = 12,
        //        Total_Amount_With_Discount = 10,
        //        OrdersDetails = new List<OrderDetails>()
        //    };


        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    //var paymentRequest = new PaymentRequest
        //    //{
        //    //    CustomerName = request.CustomerName,
        //    //    InvoiceValue = request.TotalPrice,
        //    //    CustomerEmail = request.CustomerEmail,
        //    //    CallBackUrl = "https://yourdomain.com/api/payment/callback",
        //    //    ErrorUrl = "https://yourdomain.com/payment/error"
        //    //};

        //    var paymentRequest = new PaymentRequest
        //    {
        //        CustomerName ="test",
        //        InvoiceValue = 12,
        //        CustomerEmail = "tt@gmail.com",
        //        DisplayCurrencyIso = "KWD",
        //        CallBackUrl = "https://upnq8.com/api/payment/callback",
        //        ErrorUrl = "https://upnq8.com/payment/error"
        //    };


        //    var response = await _fatoorahService.SendPaymentAsync(paymentRequest);

        //    Console.WriteLine(response.ToString());
        //    if (response.IsSuccess)
        //    {
        //        order.InvoiceId = response.Data.InvoiceId;
        //        await _context.SaveChangesAsync();

        //        return Ok(new { PaymentUrl = response.Data.InvoiceURL });
        //    }

        //    return BadRequest(response.Message);
        //}

        [HttpGet("api/getpaymentstatus")]
        public async Task<IActionResult> GetPaymentStatus([FromQuery] GetPaymentStatusRequest request)
        {
            try
            {
                var response = await _fatoorahService.GetPaymentStatusAsync(request.paymentId);
                Console.WriteLine("-----------------Data from Response GetPaymentStatus-----------------");
                Console.WriteLine(response.ToString());
                if (!response.IsSuccess)
                {
                    return BadRequest(new { message = "Failed to retrieve payment status" });
                }

                var order = await _context.Orders
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.InvoiceId == response.Data.InvoiceId);

                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                if (response.Data.InvoiceStatus == "Paid")
                {
                    order.Payment_Status_En = "Paid";
                    order.Payment_Status_Ar = "مدفوع";
                    await _context.SaveChangesAsync();

                var userName = order.User?.UserName ?? "Unknown User";
                string emailContent = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>MyFatoorah Webhook Response</h2>
                    <h3>Transaction Data</h3>
                    <table border='1' cellspacing='0' cellpadding='8' style='border-collapse: collapse; width: 100%;'>
                        <tr><th>Field</th><th>Value</th></tr>
                        {FormatDictionaryAsTable(JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(order)))}
                    </table>
                    <br/>
                    <h3>Order Data</h3>
                    <table border='1' cellspacing='0' cellpadding='8' style='border-collapse: collapse; width: 100%;'>
                        <tr><th>Field</th><th>Value</th></tr>
                        {FormatDictionaryAsTable(JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(order)))}
                    </table>
                </body>
                </html>";

                var emailMessage = new Message(userName, "support@alshamiaa.com", "Order Payment Status for Order # " + order.Id, emailContent, isHtml: true);

                if (_emailService == null)
                {
                    _logger.LogError("Email service is not initialized!");
                    return StatusCode(500, new { message = "Email service is not initialized" });
                }

                    try
                    {
                        await _emailService.SendEmail(emailMessage);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to send email: {ex.Message}");
                    }
                }
                return Ok(response);
            } catch (Exception ex)
            {
                return StatusCode(500, new { message = "Payment status failed", error = ex.Message });
            }
        }

        private object FormatDictionaryAsTable(Dictionary<string, object> data)
        {
            var sb = new StringBuilder();
            foreach (var entry in data)
            {
                sb.Append($"<tr><td><strong>{entry.Key}</strong></td><td>{entry.Value ?? "N/A"}</td></tr>");
            }
            return sb.ToString();
        }


        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromQuery] PaymentCallbackRequest request)
        {
            var statusResponse = await _fatoorahService.GetPaymentStatusAsync(request.PaymentId);

            if (!statusResponse.IsSuccess || statusResponse.Data == null)
                return BadRequest("Invalid payment status");

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.InvoiceId == statusResponse.Data.InvoiceId);

            if (order == null) return NotFound("Order not found");

            if (statusResponse.Data.InvoiceStatus == "Paid" &&
                (order.Total_Amount == statusResponse.Data.InvoiceValue ||
                 order.Total_Amount_With_Discount == statusResponse.Data.InvoiceValue ))
            {
                order.Payment_Status_Ar = "مدفوع";
                order.Payment_Status_En = "paid";

                await _context.SaveChangesAsync();
                return Redirect("https://alshamiaa.com/ar/cart?success_pay=true");
            }

            return BadRequest($"Payment verification failed Data:" +
                $" Response InoiveId: {statusResponse.Data.InvoiceId} " +
                $" Order InovideId  {order.InvoiceId}  " +
                $" Inovice Value : {statusResponse.Data.InvoiceValue}" +
                $"Order Total Amount Value {order.Total_Amount}" +
                $"Order Total_Amount_With_Discount Value {order.Total_Amount_With_Discount} ");
        }
    }

    // CheckoutRequest.cs
    public class CheckoutRequest
    {
        public string CustomerName { get; set; }
        public decimal TotalPrice { get; set; }
        public string CustomerEmail { get; set; }
        // Add other necessary properties from your order form
    }

    // PaymentCallbackRequest.cs
    public class PaymentCallbackRequest
    {
        public string PaymentId { get; set; }
    }

    // OrderStatus.cs (enum)
    public enum OrderStatus
    {
        Pending,
        Paid,
        Failed
    }



    
    public class GetPaymentStatusRequest
    {
        [JsonPropertyName("paymentId")]
        public string paymentId { get; set; }
    }

    public class GetPaymentStatusResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public PaymentData Data { get; set; }
    }

    public class PaymentData
    {
        public int InvoiceId { get; set; }
        public string InvoiceStatus { get; set; }
        public decimal InvoiceValue { get; set; }
        public string CustomerEmail { get; set; }
        public List<InvoiceTransactionModel> InvoiceTransactions { get; set; }
    }

    public class InvoiceTransactionModel
    {
        public string TransactionStatus { get; set; }
        public string PaymentId { get; set; }
    }
}
