using System;

namespace PaymentGateway.Gateway.Models.Entities
{
    public class PaymentEntity
    {
        public PaymentEntity()
        {
            Created = DateTime.UtcNow;
        }

        public int PaymentId { get; set; }
        public int CardNumber { get; set; }
        public int CVV { get; set; }
        public int ExpiryYear { get; set; }
        public int ExpiryMonth { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyISOCode { get; set; }
        public Guid MerchantId { get; set; }
        public long? BankPaymentId { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; private set; }        
        public byte[] RowVersion { get; set; }
    }
}
