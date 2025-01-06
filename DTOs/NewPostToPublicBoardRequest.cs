namespace SocialMediaAPI23Okt.DTOs
{
    public class NewPostToPublicBoardRequest
    {
        public required string Title { get; set; }

        public required string Content { get; set; }

        //public DateTime CreatedAt { get; set; }
    }
}
