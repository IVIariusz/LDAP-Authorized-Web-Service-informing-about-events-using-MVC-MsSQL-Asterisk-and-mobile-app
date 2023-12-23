using System.ComponentModel.DataAnnotations;

namespace Multimedia.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Pole 'Nick' jest wymagane.")]
        [StringLength(50, ErrorMessage = "Nick nie może przekraczać 50 znaków.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Pole 'Imię' jest wymagane.")]
        [StringLength(50, ErrorMessage = "Imię nie może przekraczać 50 znaków.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Pole 'Nazwisko' jest wymagane.")]
        [StringLength(50, ErrorMessage = "Nazwisko nie może przekraczać 50 znaków.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Pole 'Adres Email' jest wymagane.")]
        [EmailAddress(ErrorMessage = "Wprowadź prawidłowy adres email.")]
        [StringLength(100, ErrorMessage = "Adres Email nie może przekraczać 100 znaków.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pole 'Numer Telefonu' jest wymagane.")]
        [StringLength(20, ErrorMessage = "Numer Telefonu nie może przekraczać 20 znaków.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Pole 'Hasło' jest wymagane.")]
        [StringLength(100, ErrorMessage = "Hasło nie może przekraczać 100 znaków.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Pole 'Potwierdź Hasło' jest wymagane.")]
        [Compare("Password", ErrorMessage = "Hasło i potwierdzenie hasła nie pasują do siebie.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
