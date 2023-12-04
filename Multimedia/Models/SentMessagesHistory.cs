using System;
using System.Collections.Generic;

namespace Multimedia.Models
{
    public class SentMessagesHistory
    {
        public int MessageID { get; set; }
        public int UserID { get; set; }
        public int EventID { get; set; }
        public string PhoneNumber { get; set; }
        public string MessageText { get; set; }
        public DateTime SentDateTime { get; set; }
        public User User { get; set; }
    }
}
