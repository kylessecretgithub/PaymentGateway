namespace PaymentGateway.Gateway.Models
{
    public class BankResponse
    {
        public long? PaymentId { get; set; }
        public string DetailedMessage { get; set; }
        public string Status { get; set; }
    }
}
