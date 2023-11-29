using System.ComponentModel.DataAnnotations;

namespace Multimedia.Models
{
    public class UserPreference
    {
        [Key]
        public int PreferenceID { get; set; }

        // Foreign Key
        public int UserID { get; set; }

        // Preferencje
        public TimeSpan? NotificationTime { get; set; } // Oznaczony jako nullable
        [MaxLength(50)]
        public string NotificationType { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
