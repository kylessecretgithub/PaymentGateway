using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Gateway.Models;
using PaymentGateway.Gateway.Services;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService paymentService;

        public PaymentController(PaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [HttpPost("ProcessPayment")]
        public async Task<IActionResult> ProcessPaymentAsync([FromBody] PaymentRequest paymentRequest)
        {
            var response = await paymentService.SendPaymentToBankAndSaveAsync(paymentRequest);
            if (response.Status == "Processed" || response.Status == "Failed to save processed payment")
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
