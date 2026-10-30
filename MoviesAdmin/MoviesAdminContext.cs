using Microsoft.EntityFrameworkCore;

public class MoviesAdminContext(DbContextOptions<MoviesAdminContext> options) : DbContext(options)
{
    public DbSet<MoviesAdmin.Models.Movie> Movie { get; set; } = default!;
    public DbSet<MoviesAdmin.Models.Genre> Genre { get; set; } = default!;
    public DbSet<MoviesAdmin.Models.AudienceRating> AudienceRating { get; set; } = default!;
    public DbSet<MoviesAdmin.Models.CriticReview> CriticReview { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MoviesAdmin.Models.Movie>()
            .HasOne(m => m.Genre)
            .WithMany(g => g.Movies)
            .HasForeignKey(m => m.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MoviesAdmin.Models.Movie>()
            .HasOne(m => m.AudienceRating)
            .WithMany(a => a.Movies)
            .HasForeignKey(m => m.AudienceRatingId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}

