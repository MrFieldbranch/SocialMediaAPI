namespace SocialMediaAPI23Okt.DTOs
{
    public class CreateNewInterestResponse
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public bool ErrorByUser { get; set; } = false;        
    }
}
