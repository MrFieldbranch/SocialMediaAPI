namespace SocialMediaAPI23Okt.DTOs
{
    public class NewPostToPublicBoardResponse
    {
        public int Id { get; set; }

        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public bool ErrorByUser { get; set; } = false;

        public string? Title { get; set; }

        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
    }
}
