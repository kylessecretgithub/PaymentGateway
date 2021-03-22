using Newtonsoft.Json;
using System;

namespace Runner
{
    public class PaymentRequest
    {
        public int? CardNumber { get; set; }
        public int? CVV { get; set; }      
        public int? ExpiryYear { get; set; }
        public int? ExpiryMonth { get; set; }
        public decimal? Amount { get; set; }
        public string CurrencyISOCode { get; set; }
        public Guid? MerchantId { get; set; }

        public static string GetNewJson(string paymentAmountToPost)
        {
            return JsonConvert.SerializeObject(new PaymentRequest
            {
                Amount = 100,
                CardNumber = 1231231,
                CurrencyISOCode = "WOW",
                CVV = 222,
                ExpiryMonth = 10,
                ExpiryYear = 2030,
                MerchantId = Guid.Parse("10247758-5c1f-4afb-ac43-a94d2a9e5fae")
            });
        }
    }


}
