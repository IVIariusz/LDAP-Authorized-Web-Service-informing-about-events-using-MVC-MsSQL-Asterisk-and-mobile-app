using System.ComponentModel.DataAnnotations;

namespace Multimedia.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Pole 'Email' jest wymagane.")]
        [EmailAddress(ErrorMessage = "Wprowadź prawidłowy adres email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pole 'Hasło' jest wymagane.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

}
