namespace SocialMediaAPI23Okt.DTOs
{
    public class UserWithSharedInterestsResponse
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public int SharedInterestsCount { get; set; }
    }
}
