namespace PayFlowEngine.Models;

public class PaymentLog
{
    public string TransactionId { get; set; } = default!;
    public string Action { get; set; } = default!;
    public string Message { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}