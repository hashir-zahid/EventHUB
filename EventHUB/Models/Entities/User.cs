namespace EventHUB.Models.Entities
{
    
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public string Semester { get; set; }
        public string Team { get; set; }
        public string Address { get; set; }
        public string StudentID { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
