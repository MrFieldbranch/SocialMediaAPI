using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaAPI23Okt.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Interest
    {
        public int Id { get; set; }

        [MaxLength(30)]
        public required string Name { get; set; }

        public ICollection<User> Persons { get; set; } = [];
    }
}
