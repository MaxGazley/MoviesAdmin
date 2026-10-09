namespace MoviesAdmin.Models
{
    public class CriticReview
    {
        public int Id { get; set; } // unique id 
     
        public string Description { get; set; } = string.Empty;
        
        public int Rating { get; set; } // 1 - 5 stars
       
        public bool IsPublished { get; set; } // T/F if review is published
        
        public string CreatedBy { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; }
    }
}