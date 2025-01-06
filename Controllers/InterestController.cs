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
    public class InterestController : ControllerBase
    {
        private readonly InterestService _interestService;

        public InterestController(InterestService interestService)
        {
            _interestService = interestService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetInterestResponse>>> GetAllOfTheInterests()    // myUserId?????
        {
            var interests = await _interestService.GetAllOfTheInterestsAsync();

            return Ok(interests);
        }        


        [HttpGet("myowninterests")]
        public async Task<ActionResult<IEnumerable<GetInterestResponse>>> GetMyOwnInterests()
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            var interests = await _interestService.GetMyOwnInterestsAsync(myUserId);

            return Ok(interests);
        }

        [HttpGet("interestsnotownedbyme")] 
        public async Task<ActionResult<IEnumerable<GetInterestResponse>>> GetInterestsNotOwnedByMe()
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            var interests = await _interestService.GetInterestsNotOwnedByMeAsync(myUserId);

            return Ok(interests);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewInterest(CreateNewInterestRequest request)  // myUserId?????
        {
            var newInterestResponse = await _interestService.CreateNewInterestAsync(HttpContext, request);

            if (!newInterestResponse.Success)
            {
                if (newInterestResponse.ErrorByUser)
                    return BadRequest(newInterestResponse.ErrorMessage);

                return Unauthorized(newInterestResponse.ErrorMessage);
            }

            return Created($"/interest/{newInterestResponse.Id}", newInterestResponse);        
        }

        [HttpPost("{interestId:int}")]
        public async Task<IActionResult> AddInterestToMyself(int interestId)
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            var response = await _interestService.AddInterestToMyselfAsync(myUserId, interestId);

            if (!response.Success)
                return BadRequest(response.ErrorMessage);

            return Ok(); 
        }

        [HttpDelete("{interestId:int}")]
        public async Task<IActionResult> RemoveInterestFromMyself(int interestId)
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            var response = await _interestService.RemoveInterestFromMyselfAsync(myUserId, interestId);

            if (!response.Success)
                return BadRequest(response.ErrorMessage);

            return NoContent();
        }        
    }
}
