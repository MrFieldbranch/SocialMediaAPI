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

        public async Task<IEnumerable<GetInterestResponse>> GetAllOfTheInterestsAsync()
        {
            var interests = await _context.Interests.ToListAsync();
            
            var interestsResponse = interests.Select(i => new GetInterestResponse 
            {
                Id = i.Id,
                Name = i.Name
            }).ToList();

            return interestsResponse;
        }

        public async Task<IEnumerable<GetInterestResponse>> GetMyOwnInterestsAsync(int myUserId)
        {
            var myUser = await _context.Users
                .Include(u => u.Interests)
                .SingleAsync(u => u.Id == myUserId);

            return myUser.Interests.Select(i => new GetInterestResponse
            {
                Id = i.Id,
                Name = i.Name
            }).ToList();
        }

        public async Task<IEnumerable<GetInterestResponse>> GetInterestsNotOwnedByMeAsync(int myUserId)
        {
            var myInterestIds = await _context.Users
                .Where(u => u.Id == myUserId)
                .SelectMany(u => u.Interests.Select(i => i.Id))
                .ToListAsync();

            var interests = await _context.Interests
                .Where(i => !myInterestIds.Contains(i.Id))
                .ToListAsync();

            return interests.Select(i => new GetInterestResponse 
            { 
                Id= i.Id,
                Name = i.Name
            }).ToList();
        }



        public async Task<CreateNewInterestResponse> CreateNewInterestAsync(HttpContext httpContext, CreateNewInterestRequest request)
        {
            var myUserIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);            

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return new CreateNewInterestResponse
                {
                    Success = false,
                    ErrorMessage = "User ID is invalid or missing from the token."
                };

            var myUser = await _context.Users.FindAsync(myUserId);

            if (myUser == null)
                return new CreateNewInterestResponse
                {
                    Success = false,
                    ErrorMessage = "User not found."
                };

            
            var interestAlreadyExists = await _context.Interests.FirstOrDefaultAsync(i => i.Name == request.Name);

            if (interestAlreadyExists != null)
                return new CreateNewInterestResponse
                {
                    Success = false,
                    ErrorMessage = "The interest already exists.",
                    ErrorByUser = true                    
                };
            

            try
            {
                var newInterest = new Interest
                { 
                    Name = request.Name
                };

                _context.Interests.Add(newInterest);

                await _context.SaveChangesAsync();

                return new CreateNewInterestResponse
                {
                    Success = true,
                    Id = newInterest.Id,
                    Name = newInterest.Name
                };
            }
            catch
            {
                return new CreateNewInterestResponse
                {
                    Success = false,
                    ErrorMessage = "The name of the interest is too long.",
                    ErrorByUser = true
                };
            }
        }

        public async Task<OperationResponse> AddInterestToMyselfAsync(int myUserId, int interestId)
        {
            var myUser = await _context.Users
                .Include(u => u.Interests)
                .FirstOrDefaultAsync(u => u.Id == myUserId);

            if (myUser == null)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "User not found."
                };

            var interest = await _context.Interests.FindAsync(interestId);

            if (interest == null)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "Interest not found."
                };

            if (myUser.Interests.Contains(interest))
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "User already has this interest."
                };

            myUser.Interests.Add(interest);

            await _context.SaveChangesAsync();

            return new OperationResponse
            {
                Success = true
            };
        }

        public async Task<OperationResponse> RemoveInterestFromMyselfAsync(int myUserId, int interestId)
        {
            var myUser = await _context.Users
                .Include(u => u.Interests)
                .FirstOrDefaultAsync(u => u.Id == myUserId);

            if (myUser == null)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "User not found."
                };

            var interest = myUser.Interests.FirstOrDefault(i => i.Id == interestId);

            if (interest == null)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "User does not have this interest."
                };

            myUser.Interests.Remove(interest);

            await _context.SaveChangesAsync();

            return new OperationResponse
            {
                Success = true
            };
        }
    }
}
