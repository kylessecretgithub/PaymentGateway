using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Gateway.Services;
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
            var response = await reportingService.GetPaymentAsync(paymentId.Value);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }
    }
}
