using Microsoft.AspNetCore.Mvc;
using PayFlowEngine.Models;
using PayFlowEngine.Services;
using PayFlowEngine.Storage;

namespace PayFlowEngine.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService = new();

    [HttpPost("pay")]
    public IActionResult Pay(
        [FromBody] PayRequest request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey)
    {
        var result = _paymentService.Pay(
            request.CustomerId,
            request.Amount,
            request.Currency,
            idempotencyKey
        );

        return Ok(ApiResponse<Transaction>.Ok(result));
    }

    [HttpGet("status/{transactionId}")]
    public IActionResult GetStatus(string transactionId)
    {
        var result = _paymentService.GetStatus(transactionId);

        if (result == null)
            return NotFound(ApiResponse<string>.Fail("Transaction not found."));

        return Ok(ApiResponse<Transaction>.Ok(result));
    }

    [HttpPost("refund")]
    public IActionResult Refund([FromBody] RefundRequest request)
    {
        var result = _paymentService.Refund(request.TransactionId);

        if (result == null)
            return NotFound(ApiResponse<string>.Fail("Transaction not found."));

        return Ok(ApiResponse<Transaction>.Ok(result));
    }

    [HttpGet("logs")]
    public IActionResult GetLogs()
    {
        return Ok(ApiResponse<List<PaymentLog>>.Ok(InMemoryPaymentStore.Logs));
    }
}