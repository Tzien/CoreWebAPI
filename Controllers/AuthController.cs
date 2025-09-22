using CoreWebApi.Data;
using CoreWebApi.Models;
using CoreWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtTokenService _jwt;
        public AuthController(AppDbContext db, JwtTokenService jwt)
        {
            _db = db; _jwt = jwt;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), 200)]
        public IActionResult Login(LoginRequest req)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserName == req.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash) || !user.IsActive)
                return Unauthorized();

            var roles = _db.UserRoles.Where(ur => ur.UserId == user.Id)
                .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name).ToList();

            var claims = new List<Claim> {
                new Claim("sub", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("displayName", user.DisplayName ?? user.UserName)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = _jwt.Create(claims);
            return Ok(new { token, user = new { user.Id, user.UserName, user.DisplayName, roles } });
        }
    }
}