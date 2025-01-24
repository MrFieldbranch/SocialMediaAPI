using SocialMediaAPI23Okt.Enums;

namespace SocialMediaAPI23Okt.DTOs
{
    public class MessageResponse
    {
        public int Id { get; set; }

        public required string Content { get; set; }

        // Behöver jag ha ConversationId?

        public int SenderId { get; set; }

        public DateTime SentAt { get; set; }   // När måste man ha frågetecken? Efter DateTime (i detta fall).       

    }
}
