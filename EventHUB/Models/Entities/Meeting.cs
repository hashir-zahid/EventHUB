using System.ComponentModel.DataAnnotations;

namespace EventHUB.Models.Entities
{
    public class Meeting
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Meeting title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Meeting date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan Start { get; set; }

        [Required(ErrorMessage = "End time is required")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan End { get; set; }

        [Required(ErrorMessage = "Agenda is required")]
        [StringLength(500, ErrorMessage = "Agenda cannot exceed 500 characters")]
        public string Agenda { get; set; }

        [Required(ErrorMessage = "Meeting link is required")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string Link { get; set; }

        public string FormattedDate => Date.ToString("MMM dd, yyyy");
        public string FormattedTimeRange => $"{Start.ToString(@"hh\:mm")} to {End.ToString(@"hh\:mm")}";
    }
}