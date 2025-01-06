using SocialMediaAPI23Okt.Enums;

namespace SocialMediaAPI23Okt.Entities
{
    public class UserFriend
    {
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public int FriendId { get; set; }

        public User Friend { get; set; } = null!;

        public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
