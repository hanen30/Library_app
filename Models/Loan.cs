using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApp.Models
{
    public class Loan
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "La date d'emprunt est requise")]
        [DataType(DataType.Date)]
        [Display(Name = "Date d'emprunt")]
        public DateTime LoanDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date de retour")]
        public DateTime? ReturnDate { get; set; }

        // Navigation properties
        [ForeignKey("BookId")]
        public Book Book { get; set; } = null!;

        [ForeignKey("MemberId")]
        public Member Member { get; set; } = null!;
    }
}

