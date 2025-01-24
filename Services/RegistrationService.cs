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

        public async Task<bool> RegisterNewUserAsync(CreateNewUserRequest request)
        {
            var emailAlreadyExists = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (emailAlreadyExists != null) 
                return false;

            try
            {
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

                return true;
            }
            catch
            {
                throw new ArgumentException("One (or more) of 'FirstName', 'LastName', 'Email' or 'Password' is too long.");
            }
        }
    }
}
