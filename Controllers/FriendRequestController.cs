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
    public class FriendRequestController : ControllerBase
    {
        private readonly FriendRequestService _friendRequestService;

        public FriendRequestController(FriendRequestService friendRequestService)
        {
            _friendRequestService = friendRequestService;
        }

        //[HttpGet] // För att hämta alla (pending) friend requests. Jag tror inte att jag behöver ha en GET för att hämta en enskild 
        // friend request, eftersom klienten får ju userId för alla som har skickat i och med denna endpoint. Däremot så måste jag fundera på
        // om jag behöver någon extra vy i klienten, utöver AnotherUserView (eftersom den är nog anpassad för vänner/främlingar, inte däremellan).

        [HttpGet("allpendingtome")]
        public async Task<ActionResult<List<PendingFriendResponse>>> GetFriendRequestsToMe()
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            var response = await _friendRequestService.GetUsersWithPendingStatusToMe(myUserId);

            return Ok(response);
        }



        [HttpGet("allpendingfromme")]
        public async Task<ActionResult<List<PendingFriendResponse>>> GetFriendRequestsFromMe()
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            var response = await _friendRequestService.GetUsersWithPendingStatusFromMe(myUserId);

            return Ok(response);
        }

        [HttpPost("{otherUserId:int}")]  
        public async Task<IActionResult> SendFriendRequest(int otherUserId)
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            try
            {
                bool response = await _friendRequestService.SendFriendRequestAsync(myUserId, otherUserId);

                if (!response)
                    return NotFound("At least one of the users involved in this request is not found.");

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }            
        }

        [HttpPut("acceptrequest/{otherUserId:int}")]  
        public async Task<IActionResult> AcceptFriendRequest(int otherUserId)
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            try
            {
                bool response = await _friendRequestService.AcceptFriendRequestAsync(myUserId, otherUserId);

                if (!response)
                    return NotFound("At least one of the users involved in this request is not found.");

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }            
        }

        [HttpDelete("declinerequest/{otherUserId:int}")]
        public async Task<IActionResult> DeclineAndDeleteFriendRequest(int otherUserId)
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            try
            {
                bool response = await _friendRequestService.DeclineAndDeleteFriendRequestAsync(myUserId, otherUserId);

                if (!response)
                    return NotFound("At least one of the users involved in this request is not found.");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }            
        }

        [HttpDelete("withdrawrequest/{otherUserId:int}")]
        public async Task<IActionResult> WithdrawAndDeleteFriendRequest(int otherUserId)
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            try
            {
                bool response = await _friendRequestService.WithdrawAndDeleteFriendRequestAsync(myUserId, otherUserId);

                if (!response)
                    return NotFound("At least one of the users involved in this request is not found.");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }            
        }

        [HttpDelete("cancelfriendship/{otherUserId:int}")] 
        public async Task<IActionResult> CancelAndDeleteFriendship(int otherUserId) 
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            try
            {
                bool response = await _friendRequestService.CancelAndDeleteFriendshipAsync(myUserId, otherUserId);

                if (!response)
                    return NotFound("At least one of the users involved in this request is not found.");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }            
        }        
    }
}
