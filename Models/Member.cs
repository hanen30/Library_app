using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide")]
        [StringLength(100, ErrorMessage = "L'email ne peut pas dépasser 100 caractères")]
        public string Email { get; set; } = string.Empty;

        // Link to ASP.NET Identity user (optional, stores IdentityUser.Id)
        [StringLength(450)]
        public string? IdentityUserId { get; set; }

        // Navigation property pour les emprunts
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}

