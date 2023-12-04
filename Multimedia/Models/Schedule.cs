using System.ComponentModel.DataAnnotations;

namespace Multimedia.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleID { get; set; }

        public int UserID { get; set; }
        public int EventID { get; set; }
        public virtual User User { get; set; }
        public virtual Event Event { get; set; }
    }
}
