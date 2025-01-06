using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaAPI23Okt.DTOs;
using SocialMediaAPI23Okt.Services;

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
        public async Task<IActionResult> CreatePostToPublicBoard(NewPostToPublicBoardRequest request)    // myUserId?????
        {
            var newPostToPublicBoardResponse = await _postToPublicBoardService.CreatePostToPublicBoardAsync(HttpContext, request);

            if (!newPostToPublicBoardResponse.Success)
            {
                if (newPostToPublicBoardResponse.ErrorByUser)
                    return BadRequest(newPostToPublicBoardResponse.ErrorMessage);

                return Unauthorized(newPostToPublicBoardResponse.ErrorMessage);
            }

            return Ok(newPostToPublicBoardResponse);
        }
    }
}
