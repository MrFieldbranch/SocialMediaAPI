using SocialMediaAPI23Okt.Enums;

namespace SocialMediaAPI23Okt.DTOs
{
    public class CreateNewUserRequest
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Sex Sex { get; set; }
    }
}
