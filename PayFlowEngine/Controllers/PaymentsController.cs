using Microsoft.AspNetCore.Mvc;
using PayFlowEngine.Services;
using PayFlowEngine.Models;

namespace PayFlowEngine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService = new();

    [HttpPost("pay")]
    public IActionResult Pay([FromBody] PayRequest request)
    {
        try
        {
            var result = _paymentService.Pay(
                request.CustomerId,
                request.Amount,
                request.Currency
            );

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("status/{transactionId}")]
    public IActionResult GetStatus(string transactionId)
    {
        var result = _paymentService.GetStatus(transactionId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("refund")]
    public IActionResult Refund([FromBody] RefundRequest request)
    {
        try
        {
            var result = _paymentService.Refund(request.TransactionId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}