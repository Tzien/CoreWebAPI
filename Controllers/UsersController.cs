using CoreWebApi.Data;
using CoreWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) { _db = db; }

        [HttpGet]
        public ActionResult<PagedResult<object>> Get(int page = 1, int pageSize = 10, string? keyword = null)
        {
            var q = _db.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
                q = q.Where(u => u.UserName.Contains(keyword) || (u.DisplayName ?? "").Contains(keyword));
            var total = q.Count();
            var list = q.OrderByDescending(u => u.CreatedAt)
                        .Skip((page - 1) * pageSize).Take(pageSize)
                        .Select(u => new { u.Id, u.UserName, u.DisplayName, u.IsActive, u.CreatedAt })
                        .ToList();
            return Ok(new PagedResult<object> { Total = total, List = list });
        }

        [HttpPost]
        public IActionResult Create([FromBody] SysUser input)
        {
            if (_db.Users.Any(x => x.UserName == input.UserName))
                return BadRequest(new { message = "Username exists" });
            input.Id = Guid.NewGuid();
            input.PasswordHash = BCrypt.Net.BCrypt.HashPassword(string.IsNullOrWhiteSpace(input.PasswordHash) ? "Pass@123" : input.PasswordHash);
            _db.Users.Add(input);
            _db.SaveChanges();
            return Ok(new { input.Id });
        }

        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] SysUser input)
        {
            var e = _db.Users.Find(id);
            if (e == null) return NotFound();
            e.DisplayName = input.DisplayName;
            e.IsActive = input.IsActive;
            if (!string.IsNullOrWhiteSpace(input.PasswordHash))
                e.PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.PasswordHash);
            _db.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var e = _db.Users.Find(id);
            if (e == null) return NotFound();
            _db.Users.Remove(e);
            _db.SaveChanges();
            return Ok();
        }
    }
}