using SocialMediaAPI23Okt.Data;
using SocialMediaAPI23Okt.DTOs;
using System.Security.Claims;

namespace SocialMediaAPI23Okt.Services
{
    public class PersonalInfoService
    {
        private readonly ApplicationDbContext _context;

        public PersonalInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UpdatePersonalInfoResponse> UpdatePersonalInfoForUserAsync(HttpContext httpContext, UpdatePersonalInfoRequest request)
        {
            var myUserIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return new UpdatePersonalInfoResponse
                {
                    Success = false,
                    ErrorMessage = "User ID is invalid or missing from the token."
                };

            var myUser = await _context.Users.FindAsync(myUserId);

            if (myUser == null)
                return new UpdatePersonalInfoResponse
                {
                    Success = false,
                    ErrorMessage = "User not found."
                };

            myUser.PersonalInfo = request.PersonalInfo;

            await _context.SaveChangesAsync();

            return new UpdatePersonalInfoResponse
            {
                Success = true,
                //PersonalInfo = myUser.PersonalInfo
            };
                
        }
    }
}
