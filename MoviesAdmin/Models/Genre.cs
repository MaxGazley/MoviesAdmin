using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models
{
    public class Genre
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Genre")]
        public string Title { get; set; } = string.Empty;

        [ValidateNever]
        public List<Movie> Movies { get; set; } = new();
    }
}
