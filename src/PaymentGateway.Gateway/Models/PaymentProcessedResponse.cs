namespace PaymentGateway.Gateway.Models
{
    public class PaymentProcessedResponse
    {

        public PaymentProcessedResponse(int? paymentId, string status)
        {
            PaymentId = paymentId;
            Status = status;
        }

        public int? PaymentId { get; private set; }

        public string Status { get; private set; }


    }
}
