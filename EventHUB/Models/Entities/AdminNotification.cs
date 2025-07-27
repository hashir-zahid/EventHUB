namespace EventHUB.Models.Entities
{
    public class AdminNotification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
