namespace PaymentGateway.Gateway.Models
{
    public class BankResponse
    {
        public BankResponse()
        {
        }

        public BankResponse(string status)
        {
            Status = status;
        }

        public long? PaymentId { get; set; }
        public string DetailedMessage { get; set; }
        public string Status { get; set; }
    }
}
