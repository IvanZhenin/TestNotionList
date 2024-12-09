using Microsoft.IdentityModel.Tokens;
using Notion.BaseModule.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Notion.BusinessLogic.Services
{
    public class JwtProvider : IJwtProvider
    {
        public string GenerateToken(int userId, string userLogin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userLogin),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretMyKey777SuperSecretMyKey777"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
    }
}