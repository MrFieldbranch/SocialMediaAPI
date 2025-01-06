using System.ComponentModel.DataAnnotations;

namespace SocialMediaAPI23Okt.Entities
{
    public class Message
    {
        public int Id { get; set; }

        [MaxLength(1000)]
        public required string Content { get; set; }

        public DateTime SentAt { get; set; }

        public int SenderId { get; set; }

        public User Sender { get; set; } = null!; 

        public int ConversationId { get; set; }

        public Conversation Conversation { get; set; } = null!;
    }
}
