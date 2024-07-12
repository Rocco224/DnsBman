using System.ComponentModel.DataAnnotations;

namespace DnsBman.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Indirizzo email non valido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Il nome utente è obbligatorio")]
        [RegularExpression(@"^[a-zA-Z0-9-._@+]*$",
            ErrorMessage = "Il nome utente può contenere solo lettere, numeri, '-' '_', '.' '@', e '+'")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La password è obbligatoria")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "La password deve contenere almeno 6 caratteri")]
        [RegularExpression(@"^(?=.*[a-zàèéìòù])(?=.*[A-ZÀÈÉÌÒÙ])(?=.*\d)(?=.*[^a-zA-Z\d\s]).{6,}$",
            ErrorMessage = "La password deve contenere almeno una lettera minuscola, una lettera maiuscola, un numero e un carattere speciale")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Conferma password è obbligatoria")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "La password e la conferma password non corrispondono")]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; }
    }
}
