using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace UniTrade.Tools
{
    public class JwtService
    {
        /// <summary>
        /// 生成 JWT
        /// </summary>
        static public string GenerateAccessToken(string id, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {

                new Claim(ClaimTypes.Name, id),
                new Claim(ClaimTypes.Role, role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenParameter.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            var token = new JwtSecurityToken(TokenParameter.Issuer,
                    TokenParameter.Audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(TokenParameter.TokenExpiry),
                    signingCredentials: credentials);

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// 刷新 JWT
        /// </summary>
        static public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        /// 验证 JWT
        /// </summary>
        static public bool ValidateAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(TokenParameter.SecretKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = TokenParameter.Issuer,

                    ValidateAudience = true,
                    ValidAudience = TokenParameter.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateLifetime = true,
                }, out var validatedToken);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
// vim: set sw=4:
