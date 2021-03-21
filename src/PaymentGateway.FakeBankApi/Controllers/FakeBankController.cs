using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentGateway.FakeBankApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class FakeBankController : ControllerBase
    {

        [HttpPost("ProcessPayment")]
        public IActionResult ProcessPaymentAsync([FromBody] PaymentRequest paymentRequest)
        {
            if (paymentRequest.Amount == 100)
            {
                return Ok(new BankResponse
                {
                    PaymentId = 1000,
                    DetailedMessage = "Payment has been processed successfully",
                    Status = "Processed"
                });
            }
            if (paymentRequest.Amount == 1000)
            {
                return BadRequest(new BankResponse
                {
                    DetailedMessage = "Payment has been processed successfully",
                    Status = "Failed"
                });
            }
            return BadRequest();
        }
    }
}
