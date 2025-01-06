using System.ComponentModel.DataAnnotations;

namespace SocialMediaAPI23Okt.Entities
{
    public class PostToPublicBoard
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public required string Title { get; set; }

        [MaxLength(1000)]
        public required string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
