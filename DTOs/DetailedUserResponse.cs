using SocialMediaAPI23Okt.Enums;

namespace SocialMediaAPI23Okt.DTOs
{
    public class DetailedUserResponse
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public string? PersonalInfo { get; set; }

        public DateTime DateOfBirth { get; set; }

        public int? Age { get; set; }

        public Sex Sex { get; set; }
        
        public TypeOfUser TypeOfUser { get; set; } = TypeOfUser.Default;

        public List<BasicUserResponse> Friends { get; set; } = [];

        public List<GetInterestResponse> Interests { get; set; } = [];

        public List<PublicPostResponse> PublicPosts { get; set; } = [];

        public List<ConversationResponse> ConversationsAsUser1 { get; set; } = [];

        public List<ConversationResponse> ConversationsAsUser2 { get; set; } = [];

    }
}
