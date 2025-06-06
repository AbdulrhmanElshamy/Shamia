﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shamia.DataAccessLayer.Entities
{
    public class Order
    {
        public string Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Payment_Status_En { get; set; }
        public string Payment_Status_Ar { get; set; }
        public string Status_En { get; set; }
        public string Status_Ar { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public int InvoiceId { get; set; }
        public  int? DiscountId { get; set; }
        public Discount? Discount { get; set; }
        public decimal Total_Amount { get; set; }
        public string Payment_Method { get; set; }
        public decimal? Total_Amount_With_Discount { get; set; }
        public ICollection<OrderDetails> OrdersDetails { get; set; }
        public int? OrderReceiverDetailsId { get; set; }
        public OrderReceiverDetails? ReceiverDetails { get; set; }
        public int? PaymentId { get; set; } // Foreign key to Payment
        public Payment? Payment { get; set; } // Navigation property to Payment

    }
}
