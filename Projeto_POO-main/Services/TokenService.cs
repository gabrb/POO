using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FFCE.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Gera um JWT com os claims de Id, Email e Role.
        /// </summary>
        /// <param name="id">Identificador numérico do usuário (Cliente ou Produtor).</param>
        /// <param name="email">E-mail do usuário.</param>
        /// <param name="role">Role do usuário (ex: "Cliente" ou "Produtor").</param>
        /// <returns>String contendo o token JWT.</returns>
        public string GenerateToken(int id, string email, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:   _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims:   claims,
                expires:  DateTime.UtcNow.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}