using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Multimedia.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        [Required, MaxLength(50)]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }

        public int RoleID { get; set; }


        public virtual Role Role { get; set; }
        public ICollection<SentMessagesHistory> SentMessagesHistory { get; set; }

    }
}
