namespace Multimedia.Models
{
    public class EventUser
    {
        public int EventID { get; set; }
        public Event Event { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }
    }

}
