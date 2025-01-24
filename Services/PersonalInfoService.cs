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

        public async Task<bool> UpdatePersonalInfoForUserAsync(int myUserId, UpdatePersonalInfoRequest request)
        {
            var myUser = await _context.Users.FindAsync(myUserId);

            if (myUser == null)
                return false;            

            try
            {
                myUser.PersonalInfo = request.PersonalInfo;

                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                throw new ArgumentException("The content is too long.");
            }                
        }
    }
}
