namespace PayFlowEngine.Models
{
    public class Transaction
    {
        public string TransactionId { get; set; } = default!;
        public string CustomerId { get; set; } = default!;

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "AZN";

        public string Status { get; set; } = "PENDING";

        public string? BankReference { get; set; }
        public string? NetworkReference { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
