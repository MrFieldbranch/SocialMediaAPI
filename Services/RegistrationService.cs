using Microsoft.EntityFrameworkCore;
using SocialMediaAPI23Okt.Data;
using SocialMediaAPI23Okt.DTOs;
using SocialMediaAPI23Okt.Entities;

namespace SocialMediaAPI23Okt.Services
{
    public class RegistrationService
    {
        private readonly ApplicationDbContext _context;

        public RegistrationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResponse?> RegisterNewUserAsync(CreateNewUserRequest request)
        {
            var emailAlreadyExists = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (emailAlreadyExists != null) 
                return null;

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                DateOfBirth = request.DateOfBirth,
                Sex = request.Sex
            };

            _context.Users.Add(newUser);

            await _context.SaveChangesAsync();

            return new OperationResponse
            {
                Success = true
            };

            //return new NewUserResponse
            //{
            //    Id = newUser.Id,
            //    Email = newUser.Email,                
            //};
        }
    }
}
