using System;
using System.Text.Json.Serialization;

namespace PaymentGateway.Gateway.Models
{
    public class Payment
    {
        [JsonIgnore]
        public byte[] EncryptedCardNumber { get; set; }
        [JsonIgnore]
        public string CardNumber { get; set; }
        public int? CVV { get; set; }
        public int? ExpiryYear { get; set; }
        public int? ExpiryMonth { get; set; }
        public decimal? Amount { get; set; }
        public string CurrencyISOCode { get; set; }
        public Guid? MerchantId { get; set; }
        public long? BankPaymentId { get; set; }
        public string Status { get; set; }        
        public string MaskedCardNumber { get; set; }
        public void MaskCardNumber()
        {
            if (CardNumber != null)
            {
                if (CardNumber.Length < 3)
                {
                    MaskedCardNumber = CardNumber;
                    CardNumber = null;
                    return;
                }
                MaskedCardNumber = CardNumber.Substring(0, 3);
            }
            CardNumber = null;
        }
    }
}
