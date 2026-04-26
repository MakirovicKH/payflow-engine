using PayFlowEngine.Models;

namespace PayFlowEngine.Storage
{
    public static class InMemoryPaymentStore
    {
        public static List<Transaction> Transactions = new();
        public static List<PaymentLog> Logs = new();
        public static Dictionary<string, Transaction> IdempotencyKeys = new();
    }
}
