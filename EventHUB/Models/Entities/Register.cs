using System.ComponentModel.DataAnnotations;

namespace EventHUB.Models.Entities
{
    public class Register
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Student ID is required")]
        public string StudentId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public string Department { get; set; }

        [Required(ErrorMessage = "Semester is required")]
        public string Semester { get; set; }

        // Event Information
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventDate { get; set; }
        public string EventTime { get; set; }
        public string EventSpot { get; set; }
        public decimal EventPrice { get; set; }
    }
}