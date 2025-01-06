namespace SocialMediaAPI23Okt.DTOs
{
    public class PublicPostResponse
    {
        public int Id { get; set; }    

        public required string Title { get; set; }

        public required string Content { get; set; }

        public DateTime CreatedAt { get; set; }        

        public BasicUserResponse? User { get; set; }
       
    }
}
