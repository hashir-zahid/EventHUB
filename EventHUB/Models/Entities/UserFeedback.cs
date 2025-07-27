using System.ComponentModel.DataAnnotations;



namespace EventHUB.Models.Entities
{
    public class UserFeedback
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Event { get; set; }

        [Required]
        public string Feedback { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}