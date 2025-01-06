namespace SocialMediaAPI23Okt.DTOs
{
    public class PendingFriendResponse
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public DateTime RequestedAt { get; set; }
    }
}
