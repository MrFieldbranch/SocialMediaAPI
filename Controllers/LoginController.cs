using Microsoft.AspNetCore.Mvc;
using SocialMediaAPI23Okt.DTOs;
using SocialMediaAPI23Okt.Services;

namespace SocialMediaAPI23Okt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly LoginService _loginService;

        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginRequest request)
        {
            var tokenResponse = await _loginService.AuthenticateAsync(request);

            if (tokenResponse == null) 
                return Unauthorized();
            
            return Ok(tokenResponse);
        }
        
    }
}
