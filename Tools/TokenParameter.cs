namespace UniTrade.Tools
{
    public class TokenParameter
    {
        public const string Issuer = "UniTrade";
        public const string Audience = "xxx";  //受众URL或API
        public const string SecretKey = "ajfoifjweoijtrqtqhoiqhriwemnbvcxzlkjhgfdsapoiuytrewq";//秘钥
        public int AccessTokenExpirationMinutes { get; set; } = 30; // 访问令牌有效期 30分钟
        public int RefreshTokenExpirationDays { get; set; } = 7;    // 刷新令牌有效期 7天
    }
}
