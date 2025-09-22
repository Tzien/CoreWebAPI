using CoreWebApi.Data;
using CoreWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MenusController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MenusController(AppDbContext db) { _db = db; }

        [HttpGet("tree")]
        public IActionResult GetTree() => Ok(_db.Menus.OrderBy(m => m.Sort).ToList());

        [HttpPost]
        public IActionResult Create([FromBody] SysMenu m)
        {
            m.Id = Guid.NewGuid();
            _db.Menus.Add(m);
            _db.SaveChanges();
            return Ok(new { m.Id });
        }
    }
}