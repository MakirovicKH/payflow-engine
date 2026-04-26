namespace PayFlowEngine.Services
{
    public class NetworkService
    {
        public bool ConfirmTransaction()
        {
            // ÖXT/EPT payment network təsdiqini simulyasiya edir.
            // 90% uğurlu, 10% uğursuz nəticə qaytarır.
            return Random.Shared.Next(1, 11) <= 9;
        }
    }
}