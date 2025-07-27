
using System.ComponentModel.DataAnnotations;

namespace EventHUB.Models.Entities
{
    public class EventMedia
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FilePath { get; set; }

        [Required]
        [StringLength(100)]
        public string EventName { get; set; }

        [Required]
        [StringLength(20)]
        public string MediaType { get; set; } // "image" or "video"

        [Required]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(50)]
        public string FolderName { get; set; }

    }
}
