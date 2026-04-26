namespace PayFlowEngine.Models;

public class PayRequest
{
    public string CustomerId { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = default!;
}