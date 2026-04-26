using PayFlowEngine.Models;

namespace PayFlowEngine.Storage
{
    public static class InMemoryPaymentStore
    {
        public static List<Transaction> Transactions = new();
    }
}
