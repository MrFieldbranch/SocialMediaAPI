using SocialMediaAPI23Okt.Enums;

namespace SocialMediaAPI23Okt.DTOs
{
    public class ConversationResponse
    {
        public int Id { get; set; }        

        public int ConversationPartnerId { get; set; }

        public List<MessageResponse> Messages { get; set; } = [];

        public bool Success { get; set; }

        //public string? ErrorMessage { get; set; }  // Tar bort denna. Väljer 204 No Content istället.

        //public int User1Id { get; set; }   Är nog onödig. Logiken för vem som har skrivit vad kan nog vara på klientsidan.

        //public int User2Id { get; set; }   Är nog onödig. Logiken för vem som har skrivit vad kan nog vara på klientsidan.

        //public ErrorType? ErrorType { get; set; }          Behövs nog inte.

    }
}
