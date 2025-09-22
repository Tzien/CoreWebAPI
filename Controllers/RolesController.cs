using CoreWebApi.Data;
using CoreWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public RolesController(AppDbContext db) { _db = db; }

        [HttpGet]
        public IActionResult Get() => Ok(_db.Roles.ToList());

        [HttpPost]
        public IActionResult Create([FromBody] SysRole r)
        {
            r.Id = Guid.NewGuid();
            _db.Roles.Add(r);
            _db.SaveChanges();
            return Ok(new { r.Id });
        }
    }
}