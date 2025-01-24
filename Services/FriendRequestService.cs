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


        public async Task<bool> SendFriendRequestAsync(int myUserId, int otherUserId)
        {
            var users = await _context.Users.Where(u => u.Id == myUserId || u.Id == otherUserId).ToListAsync();

            if (users.Count != 2)
                return false;            

            bool friendsOrFriendRequestExists = await _context.UserFriends.AnyAsync(uf =>
                (uf.UserId == myUserId && uf.FriendId == otherUserId) || (uf.UserId == otherUserId && uf.FriendId == myUserId));

            if (friendsOrFriendRequestExists)
                throw new InvalidOperationException("You are already friends with this person, or a friend request has already been sent by one of you.");


            // Jag har ingen try/catch här, eftersom det bara var databasfel som den fångade upp.
            var newFriendRequest = new UserFriend
            {
                UserId = myUserId,
                FriendId = otherUserId,
                Status = Enums.FriendRequestStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            _context.UserFriends.Add(newFriendRequest);

            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> AcceptFriendRequestAsync(int myUserId, int otherUserId)
        {
            var users = await _context.Users.Where(u => u.Id == myUserId || u.Id == otherUserId).ToListAsync();

            if (users.Count != 2)
                return false;            

            var friendRequestRecord = await _context.UserFriends.FirstOrDefaultAsync(uf =>
                uf.UserId == otherUserId && uf.FriendId == myUserId && uf.Status == Enums.FriendRequestStatus.Pending);

            if (friendRequestRecord == null)
                throw new InvalidOperationException("No friend request exists from this user, so you cannot accept it.");

            // Jag har ingen try/catch här, eftersom det bara var databasfel som den fångade upp.
            friendRequestRecord.Status = Enums.FriendRequestStatus.Accepted;

            await _context.SaveChangesAsync();

            return true;                      
        }

        public async Task<bool> DeclineAndDeleteFriendRequestAsync(int myUserId, int otherUserId)
        {
            var users = await _context.Users.Where(u => u.Id == myUserId || u.Id == otherUserId).ToListAsync();

            if (users.Count != 2)
                return false;            

            var friendRequestRecord = await _context.UserFriends.FirstOrDefaultAsync(uf =>
                (uf.UserId == otherUserId && uf.FriendId == myUserId && uf.Status == Enums.FriendRequestStatus.Pending));

            if (friendRequestRecord == null)
                throw new InvalidOperationException("No friend request exists from this user, so you cannot decline it.");

            // Jag har ingen try/catch här, eftersom det bara var databasfel som den fångade upp.
            _context.UserFriends.Remove(friendRequestRecord);

            await _context.SaveChangesAsync();

            return true;           
        }

        public async Task<bool> WithdrawAndDeleteFriendRequestAsync(int myUserId, int otherUserId)
        {
            var users = await _context.Users.Where(u => u.Id == myUserId || u.Id == otherUserId).ToListAsync();

            if (users.Count != 2)
                return false;            

            var friendRequestRecord = await _context.UserFriends.FirstOrDefaultAsync(uf =>
                (uf.UserId == myUserId && uf.FriendId == otherUserId && uf.Status == Enums.FriendRequestStatus.Pending));

            if (friendRequestRecord == null)
                throw new InvalidOperationException("No friend request exists from you to this user.");

            // Jag har ingen try/catch här, eftersom det bara var databasfel som den fångade upp.
            _context.UserFriends.Remove(friendRequestRecord);

            await _context.SaveChangesAsync();

            return true;            
        }

        public async Task<bool> CancelAndDeleteFriendshipAsync(int myUserId, int otherUserId)
        {
            var users = await _context.Users.Where(u => u.Id == myUserId || u.Id == otherUserId).ToListAsync();

            if (users.Count != 2)
                return false;            

            var friendshipRecord = await _context.UserFriends.FirstOrDefaultAsync(uf =>
                (uf.Status == Enums.FriendRequestStatus.Accepted) && 
                ((uf.UserId == myUserId && uf.FriendId == otherUserId) || (uf.UserId == otherUserId && uf.FriendId == myUserId)));

            if (friendshipRecord == null)
                throw new InvalidOperationException("No friendship record exists with this user.");

            // Jag har ingen try/catch här, eftersom det bara var databasfel som den fångade upp.
            _context.UserFriends.Remove(friendshipRecord);

            await _context.SaveChangesAsync();

            return true;            
        }
    }
}
