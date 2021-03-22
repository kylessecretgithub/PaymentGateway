using System;

namespace PaymentGateway.Gateway.Models
{
    public class Payment
    {
        public byte[] EncryptedCardNumber { get; set; }
        public int? CVV { get; set; }
        public int? ExpiryYear { get; set; }
        public int? ExpiryMonth { get; set; }
        public decimal? Amount { get; set; }
        public string CurrencyISOCode { get; set; }
        public Guid? MerchantId { get; set; }
        public long? BankPaymentId { get; set; }
        public string Status { get; set; }
        public string CardNumber { get; set; }

        public void MaskCardNumber()
        {
            string cardNumber = CardNumber;
            if (cardNumber != null && cardNumber.Length > 2)
            {
                CardNumber = cardNumber.Substring(0, 3);
            }
        }
    }
}
