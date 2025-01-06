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
    public class MessageController : ControllerBase
    {
        private readonly MessageService _messageService;

        public MessageController(MessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("{otherUserId:int}")]
        public async Task<IActionResult> SendMessage(int otherUserId, MessageRequest messageRequest)
        {
            var myUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return Unauthorized("User ID is invalid or missing from the token.");

            var response = await _messageService.SendMessageAsync(myUserId, otherUserId, messageRequest);

            if (!response.Success)
            {
                if (response.ErrorType == Enums.ErrorType.NotFound)
                    return NotFound(response.ErrorMessage);
                else if (response.ErrorType == Enums.ErrorType.BadRequest)
                    return BadRequest(response.ErrorMessage);
                else if (response.ErrorType == Enums.ErrorType.ServerError)
                    return StatusCode(StatusCodes.Status500InternalServerError, response.ErrorMessage);
            }
            
            return Created();
        }       

    }
}
