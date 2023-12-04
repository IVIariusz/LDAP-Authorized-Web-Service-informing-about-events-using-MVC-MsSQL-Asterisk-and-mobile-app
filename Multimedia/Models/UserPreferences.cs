using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace Multimedia.Models
{
    public class ApplicationUser : IdentityUser
    {
        
    }

    public class UserPreferences
    {
        [Key]
        public string UserID { get; set; }
        public bool ReceiveMessages { get; set; }
        public TimeSpan PreferredDeliveryTime { get; set; }
        public string DeliveryMethod { get; set; }
        public TimeSpan BlockedHoursStart { get; set; }
        public TimeSpan BlockedHoursEnd { get; set; }
        public User User { get; set; }
    }

}
