using Microsoft.EntityFrameworkCore;
using SocialMediaAPI23Okt.Data;
using SocialMediaAPI23Okt.DTOs;
using SocialMediaAPI23Okt.Entities;
using SocialMediaAPI23Okt.Enums;

namespace SocialMediaAPI23Okt.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }        

        public async Task<List<BasicUserResponse>> GetMyFriendsAsync(int myUserId)  // Jag antar att jag måste ha med Include för både User och Friend
        {
            var myFriends = await _context.UserFriends
                .Include(uf => uf.User)
                .Include(uf => uf.Friend)
                .Where(uf => (uf.UserId == myUserId || uf.FriendId == myUserId) && uf.Status == FriendRequestStatus.Accepted)
                .Select(uf => uf.UserId == myUserId
                ? new BasicUserResponse { Id = uf.Friend.Id, FirstName = uf.Friend.FirstName, LastName = uf.Friend.LastName}
                : new BasicUserResponse { Id = uf.User.Id, FirstName = uf.User.FirstName, LastName = uf.User.LastName})
                .ToListAsync();

            return myFriends;
        }

        public async Task<List<BasicUserResponse>> GetStrangersAsync(int myUserId)
        {
            var excludedUserIds = await _context.UserFriends
                .Where(uf => (uf.UserId == myUserId || uf.FriendId == myUserId) &&
                (uf.Status == FriendRequestStatus.Accepted || uf.Status == FriendRequestStatus.Pending))
                .Select(uf => uf.UserId == myUserId ? uf.FriendId : uf.UserId)
                .ToListAsync();

            var strangers = await _context.Users
                .Where(u => u.Id != myUserId && !excludedUserIds.Contains(u.Id))
                .Select(u => new BasicUserResponse { Id = u.Id, FirstName = u.FirstName, LastName = u.LastName})
                .ToListAsync();

            return strangers;
        }


        // Nedanstående kan nog förenklas genom att göra query direkt till UserFriends, istället för via navigation propertyn User.FriendsRequestsFromMe och User.FriendRequestsToMe
        public async Task<DetailedUserResponse?> GetUserAsync(int myUserId, int otherUserId) 
        {
            if (myUserId == otherUserId)
            {
                var myUserResponse = await GetMySelfAsync(myUserId);

                return myUserResponse;
            }
            else
            {
                var otherUser = await _context.Users
                .Include(u => u.PostsToPublicBoard)
                .Include(u => u.Interests)
                .Include(u => u.FriendRequestsFromMe)
                .Include(u => u.FriendRequestsToMe)
                .FirstOrDefaultAsync(u => u.Id == otherUserId);

                if (otherUser == null)
                    return null;

                TypeOfUser typeOfUser = TypeOfUser.Default;

                var recordInUserFriendsTable = await _context.UserFriends.FirstOrDefaultAsync(uf =>
                (uf.UserId == myUserId && uf.FriendId == otherUserId) || (uf.UserId == otherUserId && uf.FriendId == myUserId));

                if (recordInUserFriendsTable == null)
                    typeOfUser = TypeOfUser.Stranger;
                else
                {
                    if (recordInUserFriendsTable.Status == FriendRequestStatus.Accepted)
                        typeOfUser = TypeOfUser.Friend;
                    else if (recordInUserFriendsTable.Status == FriendRequestStatus.Pending && recordInUserFriendsTable.UserId == myUserId)
                        typeOfUser = TypeOfUser.UserThatISentFriendRequestTo;
                    else if (recordInUserFriendsTable.Status == FriendRequestStatus.Pending && recordInUserFriendsTable.UserId == otherUserId)
                        typeOfUser = TypeOfUser.UserThatSentFriendRequestToMe;
                }

                //bool isFriend = await _context.UserFriends.AnyAsync(uf => 
                //    ((uf.UserId == myUserId && uf.FriendId == otherUserId) || (uf.UserId == otherUserId && uf.FriendId == myUserId)) 
                //    && uf.Status == Enums.FriendRequestStatus.Accepted); 

                var otherUserFriendIds_A = otherUser.FriendRequestsFromMe
                    .Where(f => f.Status == FriendRequestStatus.Accepted)
                    .Select(f => f.FriendId)
                    //.Select(f => f.FriendId == otherUserId ? f.UserId : f.FriendId)
                    .ToList();

                var otherUserFriendIds_B = otherUser.FriendRequestsToMe
                    .Where(f => f.Status == FriendRequestStatus.Accepted)
                    .Select(f => f.UserId)
                    .ToList();

                var otherUserTotalFriendIds = new List<int>(otherUserFriendIds_A);
                otherUserTotalFriendIds.AddRange(otherUserFriendIds_B);

                var otherUserFriends = await _context.Users
                    .Where(u => otherUserTotalFriendIds.Contains(u.Id))
                    .Select(u => new BasicUserResponse
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName
                    }).ToListAsync();
                //.ToDictionaryAsync(u => u.Id, u => new {u.FirstName, u.LastName});

                var otherUserResponse = new DetailedUserResponse
                {
                    Id = otherUser.Id,
                    FirstName = otherUser.FirstName,
                    LastName = otherUser.LastName,
                    Email = otherUser.Email,
                    PersonalInfo = typeOfUser == TypeOfUser.Friend ? otherUser.PersonalInfo : null,
                    //PersonalInfo = isFriend? otherUser.PersonalInfo : null,
                    DateOfBirth = otherUser.DateOfBirth,
                    Age = CalculateAge(otherUser.DateOfBirth),
                    Sex = otherUser.Sex,
                    TypeOfUser = typeOfUser,
                    //TypeOfUser = isFriend? Enums.TypeOfUser.Friend : Enums.TypeOfUser.Stranger,
                    //PublicPosts = otherUser.PostsToPublicBoard.Select(p => new PublicPostResponse // Behövs denna egentligen?
                    //{ 
                    //    Id = p.Id,
                    //    Title = p.Title,
                    //    Content = p.Content,
                    //    CreatedAt = p.CreatedAt
                    //}).ToList(),
                    Interests = otherUser.Interests.Select(i => new InterestResponse
                    {
                        Id = i.Id,
                        Name = i.Name
                    }).ToList(),
                    Friends = otherUserFriends
                    //Friends = otherUser.Friends.Select(f => 
                    //{
                    //    var friendId = f.FriendId == otherUserId ? f.UserId : f.FriendId;
                    //    var friendInfo = otherUserFriends[friendId];

                    //    return new BasicUserResponse
                    //    {
                    //        Id = friendId,
                    //        FirstName = friendInfo.FirstName,
                    //        LastName = friendInfo.LastName
                    //    };
                    //}).ToList()
                };

                return otherUserResponse;
            }

                        
        }

        public async Task<DetailedUserResponse?> GetMySelfAsync(int myUserId)
        {
            var myUser = await _context.Users
                .Include(u => u.PostsToPublicBoard)
                .Include(u => u.Interests)
                .Include(u => u.ConversationsAsUser1)
                .Include(u => u.ConversationsAsUser2)
                //.Include(u => u.FriendRequestsFromMe.Where(f => f.Status == FriendRequestStatus.Accepted))
                //.Include(u => u.FriendRequestsToMe)
                .FirstOrDefaultAsync(u => u.Id == myUserId);

            if (myUser == null)
                return null;            

            //var friendIds = myUser.Friends
            //    .Select(f => f.FriendId == myUserId ? f.UserId : f.FriendId)
            //    .ToList();

            //var friends = await _context.Users
            //    .Where(u => friendIds.Contains(u.Id))
            //    .Select(u => new BasicUserResponse 
            //    { 
            //        Id = u.Id, 
            //        FirstName = u.FirstName, 
            //        LastName = u.LastName 
            //    }).ToListAsync();
                //.ToDictionaryAsync(u => u.Id, u => new {u.FirstName, u.LastName});

            var myUserResponse = new DetailedUserResponse
            {
                Id = myUser.Id,
                FirstName = myUser.FirstName,
                LastName = myUser.LastName,
                Email = myUser.Email,
                PersonalInfo = myUser.PersonalInfo,
                DateOfBirth = myUser.DateOfBirth,
                Age = CalculateAge(myUser.DateOfBirth),
                Sex = myUser.Sex,
                TypeOfUser = TypeOfUser.Me,
                //PublicPosts = myUser.PostsToPublicBoard.Select(p => new PublicPostResponse 
                //{ 
                //    Id = p.Id,
                //    Title = p.Title,
                //    Content = p.Content,
                //    CreatedAt = p.CreatedAt
                //}).ToList(),
                Interests = myUser.Interests.Select(i => new InterestResponse 
                { 
                    Id = i.Id,
                    Name = i.Name
                }).ToList(),
                //ConversationsAsUser1 = myUser.ConversationsAsUser1.Select(c => new ConversationResponse 
                //{
                //    Id = c.Id,
                //    ConversationPartnerId = c.User2Id
                //}).ToList(),
                //ConversationsAsUser2 = myUser.ConversationsAsUser2.Select(c => new ConversationResponse
                //{
                //    Id= c.Id,
                //    ConversationPartnerId = c.User1Id
                //}).ToList(),
                //Friends = friends
                //Friends = myUser.Friends.Select(f =>
                //{
                //    var friendId = f.FriendId == myUserId ? f.UserId : f.FriendId;
                //    var friendInfo = friends[friendId];

                //    return new BasicUserResponse
                //    {
                //        Id = friendId,
                //        FirstName = friendInfo.FirstName,
                //        LastName = friendInfo.LastName
                //    };
                //}).ToList()
            };

            return myUserResponse;
        }


        // Nedanstående metod kan förbättras med ett HashSet<int>, men det tänker jag inte göra.
        public async Task<IEnumerable<UserWithSharedInterestsResponse>> GetStrangersSortedBySharedInterestsAsync(int myUserId)   
        {
            var myUser = await _context.Users.Include(u => u.Interests).FirstOrDefaultAsync(u => u.Id == myUserId);

            var myInterests = myUser?.Interests.Select(i => i.Id).ToList();

            if (myInterests == null || myInterests.Count == 0)
                return [];    // Om den inloggade användaren inte har några intressen, returnera en tom lista.

            var excludedUserIds = await _context.UserFriends
                .Where(uf => (uf.UserId == myUserId || uf.FriendId == myUserId) &&
                (uf.Status == FriendRequestStatus.Accepted || uf.Status == FriendRequestStatus.Pending))
                .Select(uf => uf.UserId == myUserId ? uf.FriendId : uf.UserId)
                .ToListAsync();

            var relevantUsers = await _context.Users
                .Where(u => u.Id != myUserId 
                && !excludedUserIds.Contains(u.Id) 
                && u.Interests.Any(i => myInterests.Contains(i.Id)))
                .Include(u => u.Interests).ToListAsync();

            if (relevantUsers.Count == 0)
                return [];    // Om det inte finns några andra användare alls, eller om det finns andra användare, men ingen har samma intressen.

            var relevantUsersResponse = relevantUsers.Select(u => new UserWithSharedInterestsResponse 
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                SharedInterestsCount = u.Interests.Count(i => myInterests.Contains(i.Id))
            }).OrderByDescending(x => x.SharedInterestsCount).ToList();            

            return relevantUsersResponse;
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.UtcNow;
            var age = today.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
