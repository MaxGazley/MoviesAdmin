
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;
using MoviesAdmin.Services;

[Authorize]
public class MoviesController : Controller
{
    private readonly MoviesAdminContext _context;
    private readonly AzureStorageService _azureStorage;

    //Constructor
    public MoviesController(MoviesAdminContext context, AzureStorageService azureStorage)
    {
        _context = context;
        _azureStorage = azureStorage;
    }


    // GET: MOVIES/Index
    public async Task<IActionResult> Index()
    {
        var movies = await _context.Movie
            .OrderByDescending(m => m.ReleaseDate)
            .Include(m => m.Genre)
            .Include(m => m.AudienceRating)
            .ToListAsync();

        return View(movies);
    }

    private void LoadGenreSelectList(int? selectedGenreId = null)
    {
        var genres = _context.Genre
            .OrderBy(g => g.Title)
            .ToList();

        ViewData["GenreId"] = new SelectList(genres, "Id", "Title", selectedGenreId);
    }

    private void LoadAudienceRatingSelectList(int? selectedAudienceRatingId = null)
    {
        var audienceRatings = _context.AudienceRating
            .OrderBy(a => a.Title)
            .ToList();

        ViewData["AudienceRatingId"] = new SelectList(audienceRatings, "Id", "Title", selectedAudienceRatingId);
    }


    // GET: MOVIES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie
            .Include(m => m.Genre)
            .Include(m => m.AudienceRating)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }


    // GET: MOVIES/Create
    [Authorize(Roles = "administrator")]
    public IActionResult Create()
    {
        LoadGenreSelectList();
        LoadAudienceRatingSelectList();
        return View();
    }


    // POST: MOVIES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> Create([Bind("Id,Title,RunTime,GenreId,Synopsis,AudienceRatingId,Director,InTheaters,ReleaseDate")] Movie movie, IFormFile? Poster)
    {
        ModelState.Remove(nameof(movie.ImageFileName));

        if (!ModelState.IsValid)
        {
            LoadGenreSelectList(movie.GenreId);
            LoadAudienceRatingSelectList(movie.AudienceRatingId);
            return View(movie);
        }

        if (Poster is null)
        {
            LoadGenreSelectList(movie.GenreId);
            LoadAudienceRatingSelectList(movie.AudienceRatingId);
            return View(movie);
        }

        string filename = $"{Guid.NewGuid()}{Path.GetExtension(Poster.FileName)}";
        movie.ImageFileName = await _azureStorage.UploadFileAsync(Poster, filename);

        _context.Add(movie);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    // GET: MOVIES/Edit/5
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie
            .Include(m => m.Genre)
            .Include(m => m.AudienceRating)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        LoadGenreSelectList(movie.GenreId);
        LoadAudienceRatingSelectList(movie.AudienceRatingId);
        return View(movie);
    }


    // POST: MOVIES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> Edit(int? id, IFormFile? Poster)
    {
        if (id == null)
        {
            return NotFound();
        }

        var existingMovie = await _context.Movie.FindAsync(id);
        if (existingMovie == null)
        {
            return NotFound();
        }

        if (await TryUpdateModelAsync(existingMovie, "", movie => movie.Title, movie => movie.RunTime, movie => movie.GenreId, movie => movie.Synopsis, movie => movie.AudienceRatingId, movie => movie.Director, movie => movie.InTheaters, movie => movie.ReleaseDate))
        {
            try
            {
                if (Poster is { Length: > 0 })
                {
                    string filename = $"{Guid.NewGuid()}{Path.GetExtension(Poster.FileName)}";
                    existingMovie.ImageFileName = await _azureStorage.UploadFileAsync(Poster, filename);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(existingMovie.Id))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        LoadGenreSelectList(existingMovie.GenreId);
        LoadAudienceRatingSelectList(existingMovie.AudienceRatingId);
        return View(existingMovie);
    }



    // GET: MOVIES/Delete/5
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie
            .Include(m => m.AudienceRating)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }


    // POST: MOVIES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var movie = await _context.Movie.FindAsync(id);
        if (movie != null)
        {
            _context.Movie.Remove(movie);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    private bool MovieExists(int? id)
    {
        return _context.Movie.Any(e => e.Id == id);
    }
}
