namespace MoviesAdmin.Models
{
    public class Movie
    {
        public int Id { get; set; } // unique id 

        public string Title { get; set; } = string.Empty;
        
        public int RunTime { get; set; } // in minutes
        
        public string Genre { get; set; } = string.Empty;
        
        public string Synopsis { get; set; } = string.Empty;
        
        public string Rating { get; set; } = string.Empty;

        public string Director { get; set; } = string.Empty;

        public Boolean InTheaters { get; set; } // T/F if movie is currently in theaters

        public string ImageFileName { get; set; } = string.Empty; // poster for movie
        
        public DateTime ReleaseDate { get; set; } 

    }
}
