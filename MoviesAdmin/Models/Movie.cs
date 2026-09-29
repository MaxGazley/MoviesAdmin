using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models
{
    public class Movie
    {
        [Required]
        public int Id { get; set; } // unique id 

        [StringLength(100)]
        [Required]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Run Time (minutes)")]
        [MinLength(1)]
        [Required]
        public int RunTime { get; set; } // in minutes
        
        [Required]
        public string Genre { get; set; } = string.Empty;

        [StringLength(400)]
        [Required]
        public string Synopsis { get; set; } = string.Empty;
        
  
        public string Rating { get; set; } = string.Empty;

        [Required]
        public string Director { get; set; } = string.Empty;

        [Display(Name = "In Theaters")]
        [Required]
        public Boolean InTheaters { get; set; } // T/F if movie is currently in theaters

        [Display(Name = "Poster File Name")]
        [Required]
        public string ImageFileName { get; set; } = string.Empty; // poster for movie

        [Display(Name = "Release Date")]
        [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
        [Required]
        public DateTime ReleaseDate { get; set; } 

    }
}
