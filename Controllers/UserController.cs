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
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<DetailedUserResponse>>> GetAllUsers()
        //{
        //    var allUsers = await _userService.GetAllUsersAsync();

        //    return Ok(allUsers);
        //}

        [HttpGet("myfriends")]
        public async Task<ActionResult<List<BasicUserResponse>>> GetMyFriends()
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            var myFriends = await _userService.GetMyFriendsAsync(myUserId);

            return Ok(myFriends);
        }

        [HttpGet("strangers")]
        public async Task<ActionResult<List<BasicUserResponse>>> GetStrangers()
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            var strangers = await _userService.GetStrangersAsync(myUserId);

            return Ok(strangers);
        }


        // En endpoint för att hämta info för en enskild annan användare. Då används route parameter. Denna info (userId) borde väl 
        // finnas tillgänglig i klienten. Här får jag ha ett villkor om PersonalInfo ska visas (om man är vän med personen).
        [HttpGet("{otherUserId:int}")]
        public async Task<ActionResult<DetailedUserResponse?>> GetUser(int otherUserId)  // Är det så man skriver om man vill att det ska kunna vara null?
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            var otherUser = await _userService.GetUserAsync(myUserId, otherUserId);

            if (otherUser == null)
                return NotFound("User not found.");

            return Ok(otherUser);
        }

        [HttpGet("getmyowndata")]
        public async Task<ActionResult<DetailedUserResponse?>> GetMySelf()
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            var myUser = await _userService.GetMySelfAsync(myUserId);

            if (myUser == null)
                return NotFound("User not found.");

            return Ok(myUser);
        }


        [HttpGet("getstrangersbasedoninterests")]
        public async Task<ActionResult<IEnumerable<UserWithSharedInterestsResponse>>> GetStrangersSortedBySharedInterests()
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");            

            var listOfUsers = await _userService.GetStrangersSortedBySharedInterestsAsync(myUserId);

            return Ok(listOfUsers);  // Men vad ska jag returnera om det inte finns några andra användare alls? Eller om det finns andra användare, men ingen har samma intressen. Kanske en tom lista i båda fallen.
        }


        //private int? GetMyUserIdFromClaims()
        //{
        //    var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        //    if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
        //        return null;

        //    return myUserId;
        //} 
        
    }
}
