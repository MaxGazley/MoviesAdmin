
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;
using MoviesAdmin.Services;

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
            .OrderByDescending(m => m.ReleaseDate) // order by release date descending
            .ToListAsync(); // get all movies from the database and convert to a list

        return View(movies);
    }


    // GET: MOVIES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie
            .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }


    // GET: MOVIES/Create
    public IActionResult Create()
    {
        return View();
    }


    // POST: MOVIES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,RunTime,Genre,Synopsis,Rating,Director,InTheaters,ReleaseDate")] Movie movie, IFormFile? Poster)
    {
        ModelState.Remove(nameof(movie.ImageFileName));

        if (ModelState.IsValid && Poster is not null) // vaildate input
        {
            //upload poster image to azure blob storage
            string filename = $"{Guid.NewGuid()}{Path.GetExtension(Poster.FileName)}";
            movie.ImageFileName = await _azureStorage.UploadFileAsync(Poster, filename);

            _context.Add(movie); //add new movie to the database
            await _context.SaveChangesAsync(); //save changes to the database
            return RedirectToAction(nameof(Index)); // redirect to the index page
        }
        return View(movie);
    }


    // GET: MOVIES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }
        return View(movie);
    }


    // POST: MOVIES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
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

        if (await TryUpdateModelAsync(existingMovie, "", movie => movie.Title, movie => movie.RunTime, movie => movie.Genre, movie => movie.Synopsis, movie => movie.Rating, movie => movie.Director, movie => movie.InTheaters, movie => movie.ReleaseDate))
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

        return View(existingMovie);
    }



    // GET: MOVIES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movie
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
