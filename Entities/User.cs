using Microsoft.EntityFrameworkCore;
using SocialMediaAPI23Okt.Enums;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaAPI23Okt.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public required string FirstName { get; set; }

        [MaxLength(30)]
        public required string LastName { get; set; }

        [MaxLength(50)]
        public required string Email { get; set; }

        [MaxLength(50)]
        public required string Password { get; set; }

        [MaxLength(1000)]
        public string? PersonalInfo { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Sex Sex { get; set; }

        public ICollection<UserFriend> FriendRequestsFromMe { get; set; } = [];

        public ICollection<UserFriend> FriendRequestsToMe { get; set; } = [];

        public ICollection<Interest> Interests { get; set; } = [];

        public ICollection<PostToPublicBoard> PostsToPublicBoard { get; set; } = [];

        public ICollection<Conversation> ConversationsAsUser1 { get; set; } = [];

        public ICollection<Conversation> ConversationsAsUser2 { get; set; } = [];

        public ICollection<Message> SentMessages { get; set; } = [];

    }
}
