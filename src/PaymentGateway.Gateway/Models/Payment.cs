using System;

namespace PaymentGateway.Gateway.Models
{
    public class Payment
    {
        public int? CardNumber { get; set; }
        public int? CVV { get; set; }
        public int? ExpiryYear { get; set; }
        public int? ExpiryMonth { get; set; }
        public decimal? Amount { get; set; }
        public string CurrencyISOCode { get; set; }
        public Guid? MerchantId { get; set; }
        public long? BankPaymentId { get; set; }
        public string Status { get; set; }

        public void MaskCardNumber()
        {
            string cardNumer = CardNumber.ToString();
            if (cardNumer.Length > 2)
            {
                CardNumber = int.Parse(cardNumer.Substring(0, 3));
            }
        }
    }
}
