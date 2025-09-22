using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CoreWebApi.Services
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string Key { get; set; } = default!;
        public int ExpiresMinutes { get; set; } = 120;
    }

    public class JwtTokenService
    {
        private readonly JwtOptions _opt;
        private readonly SymmetricSecurityKey _key;
        public JwtTokenService(IOptions<JwtOptions> opt)
        {
            _opt = opt.Value;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
        }

        public string Create(IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_opt.ExpiresMinutes),
                signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}