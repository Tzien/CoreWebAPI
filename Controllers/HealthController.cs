using Microsoft.AspNetCore.Mvc;

namespace CoreWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { ok = true, time = DateTime.UtcNow });
    }
}