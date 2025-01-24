using Microsoft.EntityFrameworkCore;
using SocialMediaAPI23Okt.Data;
using SocialMediaAPI23Okt.DTOs;
using SocialMediaAPI23Okt.Entities;

namespace SocialMediaAPI23Okt.Services
{
    public class MessageService
    {
        private readonly ApplicationDbContext _context;

        public MessageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MessageResponse?> SendMessageAsync(int myUserId, int otherUserId, MessageRequest messageRequest)
        {   
            var users = await _context.Users.Where(u => u.Id == myUserId || u.Id == otherUserId).ToListAsync();

            if (users.Count != 2)
                return null;                          

            var friendshipRecord = await _context.UserFriends.FirstOrDefaultAsync(uf =>
                (uf.Status == Enums.FriendRequestStatus.Accepted) && 
                ((uf.UserId == myUserId && uf.FriendId == otherUserId) || (uf.UserId == otherUserId && uf.FriendId == myUserId)));

            if (friendshipRecord == null)
                throw new InvalidOperationException("You are not friends with this user. Direct messaging requires an accepted friendship.");                
            
            var newMessage = new Message
            {
                Content = messageRequest.Content,
                SentAt = DateTime.UtcNow,
                SenderId = myUserId
            };

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingConversation = await _context.Conversations.FirstOrDefaultAsync(c =>
                (c.User1Id == myUserId && c.User2Id == otherUserId) || (c.User1Id == otherUserId && c.User2Id == myUserId));

                if (existingConversation == null)
                {
                    var newConversation = new Conversation
                    {
                        User1Id = myUserId,
                        User2Id = otherUserId,
                        Messages = new List<Message> { newMessage }
                    };

                    _context.Conversations.Add(newConversation);                    
                }
                else
                {
                    existingConversation.Messages.Add(newMessage);
                }                

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("String or binary data would be truncated") ?? false)
            {
                await transaction.RollbackAsync();

                throw new ArgumentException("Your messages is too long. It can only be max 1000 characters.");                
            }
            //catch (Exception)
            //{
            //    await transaction.RollbackAsync();

            //    throw new ApplicationException("An unexpected error occurred. Please try again later.");                
            //}
             
            return new MessageResponse
            {
                Id = newMessage.Id,
                Content = newMessage.Content,
                SenderId = newMessage.SenderId,
                SentAt = newMessage.SentAt
            };
            
        }
    }
}
