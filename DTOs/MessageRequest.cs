namespace SocialMediaAPI23Okt.DTOs
{
    public class MessageRequest
    {
        public required string Content { get; set; }

        //public DateTime SentAt { get; set; }  Behövs inte, eftersom jag kan sätta värdet "server-side", genom att använda DateTime.UtcNow.
    }
}
