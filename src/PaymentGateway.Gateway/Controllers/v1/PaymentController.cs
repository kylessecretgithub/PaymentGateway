using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Gateway.Models;
using PaymentGateway.Gateway.Services;
using Serilog;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [Authorize(Policy = "ProcessPayment")]
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
            Log.Information("Incoming payment request");
            var response = await paymentService.SendPaymentToBankAndSaveAsync(paymentRequest);
            if (response.Status == "Processed" || response.Status == "Failed to save processed payment")
            {
                Log.Information("Sucessfully processed payment to bank");
                return Ok(response);
            }
            Log.Error("Failed to process payment to bank");
            return BadRequest(response);
        }
    }
}
