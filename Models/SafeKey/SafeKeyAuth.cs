namespace PaymentProviders.Models.SafeKey
{
    public class SafeKeyAuth
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string Type { get; set; } = "MerchantSafe";
    }
}
