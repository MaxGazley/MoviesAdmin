using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models
{
    public class Movie
    {
        [Required]
        public int Id { get; set; } // unique id 

        //Navigational property to CriticReviews
        [ValidateNever]
        public List<CriticReview> CriticReviews { get; set; } = new List<CriticReview>();

        [Display(Name = "Genre")]
        [Required]
        public int GenreId { get; set; }

        //Navigational property to Genre
        [ValidateNever]
        public Genre Genre { get; set; } = null!;

        [MaxLength(100)]
        [Required]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Run Time (minutes)")]
        [Range(1, 500)]
        [Required]
        public int RunTime { get; set; } // in minutes

        [StringLength(400)]
        [Required]
        public string Synopsis { get; set; } = string.Empty;

        [Display(Name = "Audience Rating")]
        [Required]
        public int AudienceRatingId { get; set; }

        //Navigational property to AudienceRating
        [ValidateNever]
        public AudienceRating AudienceRating { get; set; } = null!;

        [Required]
        public string Director { get; set; } = string.Empty;

        [Display(Name = "In Theaters")]
        [Required]
        public Boolean InTheaters { get; set; } // T/F if movie is currently in theaters

        //URL to azure storage blob
        [Required]
        public string ImageFileName { get; set; } = string.Empty; // poster for movie

        [Display(Name = "Release Date")]
        [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
        [Required]
        public DateTime ReleaseDate { get; set; }

    }
}
