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
        public async Task<IActionResult> UpdatePersonalInfoForUser(UpdatePersonalInfoRequest request)   // myUserId?????
        {
            var updatePersonalInfoResponse = await _personalInfoService.UpdatePersonalInfoForUserAsync(HttpContext, request);

            if (!updatePersonalInfoResponse.Success)
                return Unauthorized(updatePersonalInfoResponse.ErrorMessage);            

            return Ok();            
        }
    }
}
