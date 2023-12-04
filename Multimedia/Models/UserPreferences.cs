using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.EntityFrameworkCore;


namespace Multimedia.Models
{
    public class UserPreferences
    {
        [Key]
        public int UserID { get; set; }

        [Display(Name = "Receive Messages")]
        public bool ReceiveMessages { get; set; }

        [Display(Name = "Preferred Delivery Time")]
        public TimeSpan? PreferredDeliveryTime { get; set; }

        [Display(Name = "Delivery Method")]
        [StringLength(50)]
        public string DeliveryMethod { get; set; }

        [Display(Name = "Blocked Hours Start")]
        public TimeSpan? BlockedHoursStart { get; set; }

        [Display(Name = "Blocked Hours End")]
        public TimeSpan? BlockedHoursEnd { get; set; }
    }


}
