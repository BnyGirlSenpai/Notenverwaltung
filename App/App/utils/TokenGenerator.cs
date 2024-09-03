﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace NotenverwaltungsApp.App.utils
{
    internal class TokenGenerator
    {
        public string GenerateJwtToken(string tokenJson)
        {
            var tokenData = JsonSerializer.Deserialize<TokenData>(tokenJson);

            if (tokenData == null)
            {
                throw new ArgumentException("Invalid token data");
            }

            // Define your secret key (in a real application, store this securely)
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("u3rZ8BaR5WzCnP7GdT3JPEFbL0hG5lWm5F0q9PT0Ri8=\r\n"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("FirstName", tokenData.FirstName),
                new Claim("LastName", tokenData.LastName),
                new Claim("UserId", tokenData.UserId),
                new Claim("Role", tokenData.Role),
                new Claim("Message", tokenData.Message)
            };

            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddMonths(3),  
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class TokenData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }
    }
}