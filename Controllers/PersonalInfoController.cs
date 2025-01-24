using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaAPI23Okt.DTOs;
using SocialMediaAPI23Okt.Services;
using System.Security.Claims;

namespace SocialMediaAPI23Okt.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PersonalInfoController : ControllerBase
    {
        private readonly PersonalInfoService _personalInfoService;

        public PersonalInfoController(PersonalInfoService personalInfoService)
        {
            _personalInfoService = personalInfoService;
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePersonalInfoForUser(UpdatePersonalInfoRequest request)
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            try
            {
                bool updatePersonalInfoResponse = await _personalInfoService.UpdatePersonalInfoForUserAsync(myUserId, request);

                if (!updatePersonalInfoResponse)
                    return NotFound("User not found.");

                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }                       
        }
    }
}
