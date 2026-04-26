using PayFlowEngine.Models;
using PayFlowEngine.Storage;

namespace PayFlowEngine.Services
{
    public class PaymentService
    {
        private readonly BankService _bankService = new();
        private readonly NetworkService _networkService = new();

        public Transaction Pay(string customerId, decimal amount, string currency, string? idempotencyKey)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new ArgumentException("CustomerId is required.");

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required.");

            var allowedCurrencies = new[] { "AZN", "USD", "EUR" };

            if (!allowedCurrencies.Contains(currency.ToUpper()))
                throw new ArgumentException("Currency must be AZN, USD or EUR.");

            if (!string.IsNullOrWhiteSpace(idempotencyKey) &&
                InMemoryPaymentStore.IdempotencyKeys.ContainsKey(idempotencyKey))
            {
                return InMemoryPaymentStore.IdempotencyKeys[idempotencyKey];
            }

            var transaction = new Transaction
            {
                TransactionId = $"TXN-{Guid.NewGuid()}",
                CustomerId = customerId,
                Amount = amount,
                Currency = currency.ToUpper(),
                Status = "PENDING",
                BankReference = $"BANK-{Guid.NewGuid()}",
                NetworkReference = $"EPT-{Guid.NewGuid()}",
                UpdatedAt = DateTime.UtcNow
            };

            transaction.Status = "PROCESSING";

            var bankApproved = _bankService.ProcessPayment(amount);

            if (!bankApproved)
            {
                transaction.Status = "DECLINED";
                transaction.UpdatedAt = DateTime.UtcNow;

                InMemoryPaymentStore.Logs.Add(new PaymentLog
                {
                    TransactionId = transaction.TransactionId,
                    Action = "PAY",
                    Message = "Payment declined by bank"
                });

                InMemoryPaymentStore.Transactions.Add(transaction);

                if (!string.IsNullOrWhiteSpace(idempotencyKey))
                    InMemoryPaymentStore.IdempotencyKeys[idempotencyKey] = transaction;

                return transaction;
            }

            var networkApproved = _networkService.ConfirmTransaction();

            if (!networkApproved)
            {
                transaction.Status = "FAILED";
                transaction.UpdatedAt = DateTime.UtcNow;

                InMemoryPaymentStore.Logs.Add(new PaymentLog
                {
                    TransactionId = transaction.TransactionId,
                    Action = "PAY",
                    Message = "Payment failed at payment network"
                });

                InMemoryPaymentStore.Transactions.Add(transaction);

                if (!string.IsNullOrWhiteSpace(idempotencyKey))
                    InMemoryPaymentStore.IdempotencyKeys[idempotencyKey] = transaction;

                return transaction;
            }

            transaction.Status = "SUCCESS";
            transaction.UpdatedAt = DateTime.UtcNow;

            InMemoryPaymentStore.Logs.Add(new PaymentLog
            {
                TransactionId = transaction.TransactionId,
                Action = "PAY",
                Message = "Payment completed successfully"
            });

            InMemoryPaymentStore.Transactions.Add(transaction);

            if (!string.IsNullOrWhiteSpace(idempotencyKey))
                InMemoryPaymentStore.IdempotencyKeys[idempotencyKey] = transaction;

            return transaction;
        }

        public Transaction? GetStatus(string transactionId)
        {
            return InMemoryPaymentStore.Transactions
                .FirstOrDefault(x => x.TransactionId == transactionId);
        }

        public Transaction? Refund(string transactionId)
        {
            var transaction = GetStatus(transactionId);

            if (transaction == null)
                return null;

            if (transaction.Status != "SUCCESS")
                throw new ArgumentException("Only successful and unrefunded transactions can be refunded.");

            transaction.Status = "REFUNDED";
            transaction.UpdatedAt = DateTime.UtcNow;

            InMemoryPaymentStore.Logs.Add(new PaymentLog
            {
                TransactionId = transaction.TransactionId,
                Action = "REFUND",
                Message = "Payment refunded successfully"
            });

            return transaction;
        }
    }
}