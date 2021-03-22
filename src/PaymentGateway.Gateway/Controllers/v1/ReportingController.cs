using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Gateway.Services;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace PaymentGateway.Gateway.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ReportingController : ControllerBase
    {
        private readonly ReportingService reportingService;

        public ReportingController(ReportingService reportingService)
        {
            this.reportingService = reportingService;
        }

        [HttpGet("GetPayment")]
        public async Task<IActionResult> GetPaymentAsync([Required]int? paymentId)
        {
            Log.Information($"Incoming GetPaymentRequest with payment ID {paymentId}");
            var response = await reportingService.GetPaymentAsync(paymentId.Value);
            if (response == null)
            {
                Log.Error($"No paymentId found with ID {paymentId}");
                return NotFound();
            }
            Log.Information($"Returning payment with payment ID {paymentId}");
            return Ok(response);
        }
    }
}
