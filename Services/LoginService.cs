using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialMediaAPI23Okt.Data;
using SocialMediaAPI23Okt.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SocialMediaAPI23Okt.Services
{
    public class LoginService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public LoginService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TokenResponse?> AuthenticateAsync(UserLoginRequest request)
        {
            var myUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

            if (myUser == null)
                return null;

            var signingKey = Convert.FromBase64String(_configuration["JWT:SigningSecret"] ?? throw new InvalidOperationException("JWT SigningSecret is not configured."));

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, myUser.Id.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(claims)
            };

            var jwtHandler = new JwtSecurityTokenHandler();

            var jwtSecurityToken = jwtHandler.CreateJwtSecurityToken(tokenDescriptor);

            var tokenResponse = new TokenResponse
            { 
                Token = jwtHandler.WriteToken(jwtSecurityToken),
                FirstName = myUser.FirstName,
                LastName = myUser.LastName
            };

            return tokenResponse;
        }
    }
}
