using Microsoft.EntityFrameworkCore;
using SocialMediaAPI23Okt.Data;
using SocialMediaAPI23Okt.DTOs;
using SocialMediaAPI23Okt.Entities;

namespace SocialMediaAPI23Okt.Services
{
    public class FriendRequestService
    {
        private readonly ApplicationDbContext _context;

        public FriendRequestService(ApplicationDbContext context)
        {
            _context = context;
        }


        //public async Task<IEnumerable<KanskeSkapaEnNyDTO>> GetAllFriendRequestsAsync(int myUserId)
        //{

        //}


        public async Task<List<PendingFriendResponse>> GetUsersWithPendingStatusFromMe(int myUserId)
        {
            var relevantUsers = await _context.UserFriends
                .Where(uf => uf.UserId == myUserId && uf.Status == Enums.FriendRequestStatus.Pending)
                .Select(uf => new
                {
                    uf.FriendId,
                    uf.RequestedAt,
                    FriendFirstName = uf.Friend.FirstName,
                    FriendLastName = uf.Friend.LastName
                })
                .ToListAsync();

            var relevantUsersResponse = relevantUsers.Select(u => new PendingFriendResponse 
            { 
                Id = u.FriendId,
                FirstName = u.FriendFirstName,
                LastName = u.FriendLastName,
                RequestedAt = u.RequestedAt
            }).ToList();

            return relevantUsersResponse;
        }

        public async Task<List<PendingFriendResponse>> GetUsersWithPendingStatusToMe(int myUserId)
        {
            var relevantUsers = await _context.UserFriends
                .Where(uf => uf.FriendId == myUserId && uf.Status == Enums.FriendRequestStatus.Pending)
                .Select(uf => new
                {
                    uf.UserId,
                    uf.RequestedAt,
                    FriendFirstName = uf.User.FirstName,
                    FriendLastName = uf.User.LastName
                })
                .ToListAsync();

            var relevantUsersResponse = relevantUsers.Select(u => new PendingFriendResponse
            {
                Id = u.UserId,
                FirstName = u.FriendFirstName,
                LastName = u.FriendLastName,
                RequestedAt = u.RequestedAt
            }).ToList();

            return relevantUsersResponse;
        }


        public async Task<OperationResponse> SendFriendRequestAsync(int myUserId, int otherUserId)
        {
            var myUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == myUserId);

            var otherUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == otherUserId);

            if (myUser == null || otherUser == null)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "At least one of the users involved in this request does not exist."
                };

            bool friendsOrFriendRequestExists = await _context.UserFriends.AnyAsync(uf =>
                (uf.UserId == myUserId && uf.FriendId == otherUserId) || (uf.UserId == otherUserId && uf.FriendId == myUserId));

            if (friendsOrFriendRequestExists)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "You are already friends with this person, or a friend request has already been sent by one of you."
                };

            try
            {
                var newFriendRequest = new UserFriend
                {
                    UserId = myUserId,
                    FriendId = otherUserId,
                    Status = Enums.FriendRequestStatus.Pending,
                    RequestedAt = DateTime.UtcNow
                };

                _context.UserFriends.Add(newFriendRequest);                               

                await _context.SaveChangesAsync();

                return new OperationResponse
                {
                    Success = true
                };
            }
            catch
            {
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "Database error. Please try again later."
                };
            }
        }


        public async Task<OperationResponse> AcceptFriendRequestAsync(int myUserId, int otherUserId)
        {
            var myUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == myUserId);

            var otherUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == otherUserId);

            if (myUser == null || otherUser == null)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "At least one of the users in this request does not exist.",
                    ErrorType = Enums.ErrorType.NotFound
                };

            var friendRequestRecord = await _context.UserFriends.FirstOrDefaultAsync(uf =>
                uf.UserId == otherUserId && uf.FriendId == myUserId && uf.Status == Enums.FriendRequestStatus.Pending);

            if (friendRequestRecord == null)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "No friend request exists from this user.",
                    ErrorType = Enums.ErrorType.NotFound
                };

            try
            {
                friendRequestRecord.Status = Enums.FriendRequestStatus.Accepted;                

                await _context.SaveChangesAsync();

                return new OperationResponse
                {
                    Success = true
                };
            }
            catch
            {
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "Database error. Please try again later.",
                    ErrorType = Enums.ErrorType.ServerError
                };
            }            
        }

        public async Task<OperationResponse> DeclineAndDeleteFriendRequestAsync(int myUserId, int otherUserId)
        {
            var myUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == myUserId);

            var otherUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == otherUserId);

            if (myUser == null || otherUser == null)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "At least one of the users in this request does not exist.",
                    ErrorType = Enums.ErrorType.NotFound
                };

            var friendRequestRecord = await _context.UserFriends.FirstOrDefaultAsync(uf =>
                (uf.UserId == otherUserId && uf.FriendId == myUserId && uf.Status == Enums.FriendRequestStatus.Pending));

            if (friendRequestRecord == null)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "No friend request exists from this user.",
                    ErrorType = Enums.ErrorType.NotFound
                };

            try
            {
                _context.UserFriends.Remove(friendRequestRecord);                

                await _context.SaveChangesAsync();

                return new OperationResponse
                {
                    Success = true
                };
            }
            catch
            {
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "Database error. Please try again later.",
                    ErrorType = Enums.ErrorType.ServerError
                };
            }
        }

        public async Task<OperationResponse> WithdrawAndDeleteFriendRequestAsync(int myUserId, int otherUserId)
        {
            var myUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == myUserId);

            var otherUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == otherUserId);

            if (myUser == null || otherUser == null)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "At least one of the users in this request does not exist.",
                    ErrorType = Enums.ErrorType.NotFound
                };

            var friendRequestRecord = await _context.UserFriends.FirstOrDefaultAsync(uf =>
                (uf.UserId == myUserId && uf.FriendId == otherUserId && uf.Status == Enums.FriendRequestStatus.Pending));

            if (friendRequestRecord == null)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "No friend request exists from you to this user.",
                    ErrorType = Enums.ErrorType.NotFound
                };

            try
            {
                _context.UserFriends.Remove(friendRequestRecord);                

                await _context.SaveChangesAsync();

                return new OperationResponse
                {
                    Success = true
                };
            }
            catch
            {
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "Database error. Please try again later.",
                    ErrorType = Enums.ErrorType.ServerError
                };
            }
        }

        public async Task<OperationResponse> CancelAndDeleteFriendshipAsync(int myUserId, int otherUserId)
        {
            var myUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == myUserId);

            var otherUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == otherUserId);

            if (myUser == null || otherUser == null)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "At least one of the users in this request does not exist.",
                    ErrorType = Enums.ErrorType.NotFound
                };

            var friendshipRecord = await _context.UserFriends.FirstOrDefaultAsync(uf =>
                (uf.Status == Enums.FriendRequestStatus.Accepted) && ((uf.UserId == myUserId && uf.FriendId == otherUserId) || (uf.UserId == otherUserId && uf.FriendId == myUserId)));

            if (friendshipRecord == null)
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "No friendship record exists with this user.",
                    ErrorType = Enums.ErrorType.NotFound
                };

            try
            {
                _context.UserFriends.Remove(friendshipRecord);                

                await _context.SaveChangesAsync();

                return new OperationResponse
                {
                    Success = true
                };
            }
            catch
            {
                return new OperationResponse
                {
                    Success = false,
                    ErrorMessage = "Database error. Please try again later.",
                    ErrorType = Enums.ErrorType.ServerError
                };
            }
        }
    }
}
