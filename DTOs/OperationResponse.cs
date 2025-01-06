using SocialMediaAPI23Okt.Enums;

namespace SocialMediaAPI23Okt.DTOs
{
    public class OperationResponse
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public ErrorType? ErrorType { get; set; }
    }
}
