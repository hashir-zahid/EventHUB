using System.ComponentModel.DataAnnotations;

namespace EventHUB.Models.Entities
{
    public class MemberMessage
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string StudentID { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}