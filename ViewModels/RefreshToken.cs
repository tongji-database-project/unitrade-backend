namespace UniTrade.Models
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsRevoked { get; set; } = false;
    }
}