using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models
{
    public class CriticReview
    {
        [Required]
        public int Id { get; set; } // unique id 

        public int MovieId { get; set; } // foreign key to Movie
        
        public Movie Movie { get; set; } = null!; // Navigational property to Movie

        [Required]
        [StringLength(500, ErrorMessage = "Review cannot exceed 500 characters.")]
        public string Review { get; set; } = string.Empty;

        [Required(ErrorMessage = "A rating is required.")]
        [Range(0, 100, ErrorMessage = "Rating must be between 0 and 100.")]
        public int Rating { get; set; } // 0-100 % 

        [Required]
        [Display(Name = "Publish to site")]
        public bool IsPublished { get; set; } // T/F if review is published

        [Required]
        public string CreatedBy { get; set; } = string.Empty; // auth0 user id of reviewer

        [Required]
        [ScaffoldColumn(false)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}