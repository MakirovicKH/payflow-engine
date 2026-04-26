namespace PayFlowEngine.Services
{
    public class BankService
    {
        public bool ProcessPayment(decimal amount)
        {
            // fraud sistem,1000 den boyuk meblegde risk oldugunu gosterir
            if (amount > 1000)
            {
                // 50% shans ile reject alinir
                return Random.Shared.Next(0, 2) == 1;
            }

            // kicik meblegler hemise keçir.
            return true;
        }
    }
}