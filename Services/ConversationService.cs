using Microsoft.EntityFrameworkCore;
using SocialMediaAPI23Okt.Data;
using SocialMediaAPI23Okt.DTOs;

namespace SocialMediaAPI23Okt.Services
{
    public class ConversationService
    {
        private readonly ApplicationDbContext _context;

        public ConversationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ConversationResponse> GetConversationAsync(int myUserId, int otherUserId)
        {
            var conversationRecord = await _context.Conversations.Include(c => c.Messages).FirstOrDefaultAsync(c =>
                (c.User1Id == myUserId && c.User2Id == otherUserId) || (c.User1Id == otherUserId && c.User2Id == myUserId));

            if (conversationRecord == null)
                return new ConversationResponse
                {
                    Success = false                                       
                };

            var response = new ConversationResponse
            {
                Success = true,
                Id = conversationRecord.Id,
                ConversationPartnerId = conversationRecord.User1Id == otherUserId ? conversationRecord.User1Id : conversationRecord.User2Id,                
                Messages = conversationRecord.Messages.Select(m => new MessageResponse
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderId = m.SenderId,
                    SentAt = m.SentAt          // Måste jag explicit sortera i kronologisk ordning?
                }).ToList()
            };

            return response;            
        }
    }
}
