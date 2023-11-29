using System.ComponentModel.DataAnnotations;

namespace Multimedia.Models
{
    public class EventRegistration
    {
        [Key]
        public int RegistrationID { get; set; }

        // Foreign Keys
        public int UserID { get; set; }
        public int EventID { get; set; }
        [Required]
        public DateTime RegistrationTime { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Event Event { get; set; }
    }
}
