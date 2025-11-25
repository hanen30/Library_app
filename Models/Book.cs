using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est requis")]
        [StringLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'auteur est requis")]
        [StringLength(100, ErrorMessage = "L'auteur ne peut pas dépasser 100 caractères")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'année est requise")]
        [Range(1000, 3000, ErrorMessage = "L'année doit être entre 1000 et 3000")]
        public int Year { get; set; }

        [StringLength(50, ErrorMessage = "Le genre ne peut pas dépasser 50 caractères")]
        public string? Genre { get; set; }

        // Navigation property pour les emprunts
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}

