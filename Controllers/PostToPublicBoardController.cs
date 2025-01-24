using Azure;
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
    public class PostToPublicBoardController : ControllerBase
    {
        private readonly PostToPublicBoardService _postToPublicBoardService;

        public PostToPublicBoardController(PostToPublicBoardService postToPublicBoardService)
        {
            _postToPublicBoardService = postToPublicBoardService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicPostResponse>>> GetAllPostsFromAllUsers()
        {
            var allPosts = await _postToPublicBoardService.GetAllPostsFromAllUsersAsync();
            return Ok(allPosts);
        }


        [HttpPost]
        public async Task<IActionResult> CreatePostToPublicBoard(NewPostToPublicBoardRequest request)
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            try
            {
                var newPostToPublicBoardResponse = await _postToPublicBoardService.CreatePostToPublicBoardAsync(myUserId, request);

                if (newPostToPublicBoardResponse == null)
                    return NotFound("User not found.");

                return Created($"/posttopublicboard/{newPostToPublicBoardResponse.Id}", newPostToPublicBoardResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }            
        }
    }
}
