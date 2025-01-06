using Microsoft.EntityFrameworkCore;
using SocialMediaAPI23Okt.Data;
using SocialMediaAPI23Okt.DTOs;
using SocialMediaAPI23Okt.Entities;
using System.Security.Claims;

namespace SocialMediaAPI23Okt.Services
{
    public class PostToPublicBoardService
    {
        private readonly ApplicationDbContext _context;

        public PostToPublicBoardService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<PublicPostResponse>> GetAllPostsFromAllUsersAsync()
        {
            var allPosts = await _context.PostsToPublicBoard.Include(p => p.User).ToListAsync();

            if (allPosts.Count == 0)
                return [];

            var allPostsResponse = allPosts.Select(a => new PublicPostResponse 
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                CreatedAt = a.CreatedAt,                
                User = new BasicUserResponse
                {
                    Id = a.User.Id,
                    FirstName = a.User.FirstName,
                    LastName = a.User.LastName
                }
            }).ToList();            

            return allPostsResponse;
        }


        public async Task<NewPostToPublicBoardResponse> CreatePostToPublicBoardAsync(HttpContext httpContext, NewPostToPublicBoardRequest request)
        {
            var myUserIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (myUserIdClaim == null || !int.TryParse(myUserIdClaim.Value, out var myUserId))
                return new NewPostToPublicBoardResponse
                {
                    Success = false,
                    ErrorMessage = "User ID is invalid or missing from the token."
                };

            var myUser = await _context.Users.FindAsync(myUserId);

            if (myUser == null)
                return new NewPostToPublicBoardResponse
                {
                    Success = false,
                    ErrorMessage = "User not found."
                };

            try
            {
                var newPostToPublicBoard = new PostToPublicBoard
                {
                    Title = request.Title,
                    Content = request.Content,
                    CreatedAt = DateTime.UtcNow,
                    UserId = myUser.Id
                };

                _context.PostsToPublicBoard.Add(newPostToPublicBoard);                

                await _context.SaveChangesAsync();

                return new NewPostToPublicBoardResponse
                {
                    Id = newPostToPublicBoard.Id,
                    Success = true,
                    Title = newPostToPublicBoard.Title,
                    Content = newPostToPublicBoard.Content,
                    CreatedAt = newPostToPublicBoard.CreatedAt,
                    UserId = newPostToPublicBoard.UserId
                };
            }
            catch
            {
                return new NewPostToPublicBoardResponse
                {
                    Success = false,
                    ErrorMessage = "The title or/and the content is too long.",
                    ErrorByUser = true
                };
            }
        }
    }
}
