using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models
{
    public class UserSnapshot
    {
        [Key]
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
