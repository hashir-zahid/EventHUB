using System.ComponentModel.DataAnnotations;

namespace EventHUB.Models.Entities
{
    public class Event
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan Start { get; set; }

        [Required]
        public TimeSpan End { get; set; }

        public string Description { get; set; }

        [Required]
        public string Spot { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 10000)]
        public int Max_Attendees { get; set; }

    }
}
