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
        public async Task<ActionResult<IEnumerable<InterestResponse>>> GetAllOfTheInterests()    // myUserId?????
        {
            var interests = await _interestService.GetAllOfTheInterestsAsync();

            return Ok(interests);
        }        


        [HttpGet("myowninterests")]
        public async Task<ActionResult<IEnumerable<InterestResponse>>> GetMyOwnInterests()
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            var interests = await _interestService.GetMyOwnInterestsAsync(myUserId);

            return Ok(interests);
        }

        [HttpGet("interestsnotownedbyme")] 
        public async Task<ActionResult<IEnumerable<InterestResponse>>> GetInterestsNotOwnedByMe()
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            var interests = await _interestService.GetInterestsNotOwnedByMeAsync(myUserId);

            return Ok(interests);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewInterest(CreateNewInterestRequest request)
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            try
            {
                var newInterestResponse = await _interestService.CreateNewInterestAsync(myUserId, request);
                if (newInterestResponse == null)
                    return NotFound("User not found.");

                return Created($"/interest/{newInterestResponse.Id}", newInterestResponse);

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);

            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);       
            }                     
        }

        [HttpPost("{interestId:int}")]
        public async Task<IActionResult> AddInterestToMyself(int interestId)
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            try
            {
                bool response = await _interestService.AddInterestToMyselfAsync(myUserId, interestId);

                if (!response)
                    return NotFound("One (or both) of either the user or the interest is not found.");

                return Ok();
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            } 
        }

        [HttpDelete("{interestId:int}")]
        public async Task<IActionResult> RemoveInterestFromMyself(int interestId)
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");
            try
            {
                bool response = await _interestService.RemoveInterestFromMyselfAsync(myUserId, interestId);

                if (!response)
                    return NotFound("User not found.");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }            
        }        
    }
}
