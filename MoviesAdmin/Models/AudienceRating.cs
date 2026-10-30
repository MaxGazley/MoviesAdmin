using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models
{
    public class AudienceRating
    {
        [Required]
        public int Id { get; set; }

        [ValidateNever]
        public List<Movie> Movies { get; set; } = new();

        [Required]
        [Display(Name = "Audience Rating")]
        public string Title { get; set; } = string.Empty;
    }
}
