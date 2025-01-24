using Microsoft.EntityFrameworkCore;
using SocialMediaAPI23Okt.Data;
using SocialMediaAPI23Okt.DTOs;
using SocialMediaAPI23Okt.Entities;
using System.Security.Claims;

namespace SocialMediaAPI23Okt.Services
{
    public class InterestService
    {
        private readonly ApplicationDbContext _context;

        public InterestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InterestResponse>> GetAllOfTheInterestsAsync()
        {
            var interests = await _context.Interests.ToListAsync();
            
            var interestsResponse = interests.Select(i => new InterestResponse 
            {
                Id = i.Id,
                Name = i.Name
            }).ToList();

            return interestsResponse;
        }

        public async Task<IEnumerable<InterestResponse>> GetMyOwnInterestsAsync(int myUserId)
        {
            var myUser = await _context.Users
                .Include(u => u.Interests)
                .SingleAsync(u => u.Id == myUserId);

            return myUser.Interests.Select(i => new InterestResponse
            {
                Id = i.Id,
                Name = i.Name
            }).ToList();
        }

        public async Task<IEnumerable<InterestResponse>> GetInterestsNotOwnedByMeAsync(int myUserId)
        {
            var myInterestIds = await _context.Users
                .Where(u => u.Id == myUserId)
                .SelectMany(u => u.Interests.Select(i => i.Id))
                .ToListAsync();

            var interests = await _context.Interests
                .Where(i => !myInterestIds.Contains(i.Id))
                .ToListAsync();

            return interests.Select(i => new InterestResponse 
            { 
                Id= i.Id,
                Name = i.Name
            }).ToList();
        }



        public async Task<InterestResponse?> CreateNewInterestAsync(int myUserId, CreateNewInterestRequest request)
        {
            var myUser = await _context.Users.FindAsync(myUserId);

            if (myUser == null)
                return null;
            
            var interestAlreadyExists = await _context.Interests.FirstOrDefaultAsync(i => i.Name == request.Name);

            if (interestAlreadyExists != null)
                throw new InvalidOperationException("The interest already exists.");             

            try
            {
                var newInterest = new Interest
                { 
                    Name = request.Name
                };

                _context.Interests.Add(newInterest);
                await _context.SaveChangesAsync();

                return new InterestResponse
                {                    
                    Id = newInterest.Id,
                    Name = newInterest.Name
                };
            }
            catch
            {
                throw new ArgumentException("The name of the interest is too long.");                
            }
        }

        public async Task<bool> AddInterestToMyselfAsync(int myUserId, int interestId)
        {
            var myUser = await _context.Users
                .Include(u => u.Interests)
                .FirstOrDefaultAsync(u => u.Id == myUserId);

            if (myUser == null)
                return false;                

            var interest = await _context.Interests.FindAsync(interestId);

            if (interest == null)
                return false;                

            if (myUser.Interests.Contains(interest))
            {
                throw new InvalidOperationException("User already has this interest.");
            }                

            myUser.Interests.Add(interest);

            await _context.SaveChangesAsync();

            return true;            
        }

        public async Task<bool> RemoveInterestFromMyselfAsync(int myUserId, int interestId)
        {
            var myUser = await _context.Users
                .Include(u => u.Interests)
                .FirstOrDefaultAsync(u => u.Id == myUserId);

            if (myUser == null)
                return false;                

            var interest = myUser.Interests.FirstOrDefault(i => i.Id == interestId);

            if (interest == null)
                throw new InvalidOperationException("User does not have this interest.");                

            myUser.Interests.Remove(interest);

            await _context.SaveChangesAsync();

            return true;            
        }
    }
}
