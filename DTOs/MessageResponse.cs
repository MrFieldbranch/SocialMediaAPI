using SocialMediaAPI23Okt.Enums;

namespace SocialMediaAPI23Okt.DTOs
{
    public class MessageResponse
    {
        public int Id { get; set; }

        public string? Content { get; set; }

        // Behöver jag ha ConversationId?

        public int SenderId { get; set; }

        public DateTime SentAt { get; set; }   // När måste man ha frågetecken? Efter DateTime (i detta fall).

        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public ErrorType? ErrorType { get; set; }

    }
}
