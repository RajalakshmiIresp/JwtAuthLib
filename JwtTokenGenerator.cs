using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthLib
{
    public static class JwtTokenGenerator
    {
        public static string GenerateJwtToken(string issuer, string audience, string signingKey, string userId, string userName, int expirationHours)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),   
                new Claim(JwtRegisteredClaimNames.Name, userName), 
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64), // Issued at time
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddMinutes(expirationHours).ToString(), ClaimValueTypes.String) // Expiration time
            };

            // Create the JWT token with a configurable expiration
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationHours),
                signingCredentials: credentials
            );

            // Generate the JWT token string
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
