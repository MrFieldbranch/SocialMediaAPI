using Microsoft.AspNetCore.Mvc;

namespace SocialMediaAPI23Okt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("API is running");
    }
}
