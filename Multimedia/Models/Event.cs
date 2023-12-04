using System.ComponentModel.DataAnnotations;

namespace Multimedia.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }
        [Required, MaxLength(100)]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime StartDateTime { get; set; }
        [Required]
        public DateTime EndDateTime { get; set; }
        [MaxLength(100)]
        public string Location { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
