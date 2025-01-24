using Microsoft.AspNetCore.Mvc;
using SocialMediaAPI23Okt.DTOs;
using SocialMediaAPI23Okt.Services;

namespace SocialMediaAPI23Okt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationService _registrationService;

        public RegistrationController(RegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewUser(CreateNewUserRequest request)
        {
            try
            {
                bool newUserResponse = await _registrationService.RegisterNewUserAsync(request);

                if (!newUserResponse)
                    return BadRequest("There is already a user with this email registered.");

                return Created();   // Jag tror att det är ok att det är tomt innanför parenteserna.

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }            
        }
    }
}
