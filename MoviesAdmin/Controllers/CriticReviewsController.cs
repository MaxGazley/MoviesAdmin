using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;

[Authorize(Roles = "administrator, critic")]
public class CriticReviewsController : Controller
{
    private readonly MoviesAdminContext _context;

    private string? _userId;

    public CriticReviewsController(MoviesAdminContext context)
    {
        _context = context;
    }

    //user object not available when constructor is called, so we need to override OnActionExecuting to get the user id
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        _userId = User.GetUserId();
    }

    public async Task<IActionResult> Index()
    {
        var criticReviews = await _context.CriticReview
            .Where(review => review.CreatedBy == _userId)
            .Include(review => review.Movie)
            .OrderBy(review => review.Movie.Title)
            .ThenByDescending(review => review.CreatedDate)
            .ToListAsync();

        return View(criticReviews);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var criticReview = await _context.CriticReview
            .Where(review => review.CreatedBy == _userId)
            .Include(review => review.Movie)
            .FirstOrDefaultAsync(review => review.Id == id);

        if (criticReview == null || criticReview.CreatedBy != _userId)
        {
            return NotFound();
        }

        return View(criticReview);
    }

    public async Task<IActionResult> Create(int? movieId)
    {
        var selectedMovie = movieId.HasValue
            ? await _context.Movie.FirstOrDefaultAsync(movie => movie.Id == movieId.Value)
            : null;

        await LoadMovieSelectList(selectedMovie?.Id);
        ViewData["MovieTitle"] = selectedMovie?.Title;
        return View(new CriticReview { MovieId = selectedMovie?.Id ?? 0 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MovieId,Review,Rating,IsPublished")] CriticReview criticReview, string? movieTitle)
    {
        ModelState.Remove(nameof(CriticReview.CreatedBy));
        criticReview.CreatedBy = _userId;

        if (!await _context.Movie.AnyAsync(movie => movie.Id == criticReview.MovieId))
        {
            ModelState.AddModelError(nameof(CriticReview.MovieId), "Select a valid movie.");
        }

        if (ModelState.IsValid)
        {
            _context.Add(criticReview);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        await LoadMovieSelectList(criticReview.MovieId);
        ViewData["MovieTitle"] = movieTitle;
        return View(criticReview);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var criticReview = await _context.CriticReview
            .Where(review => review.Id == id)
            .FirstOrDefaultAsync();

        if (criticReview == null || criticReview.CreatedBy != _userId)
        {
            return NotFound();
        }

        await LoadMovieSelectList(criticReview.MovieId);
        ViewData["MovieTitle"] = ((List<Movie>)ViewData["Movies"]!).FirstOrDefault(movie => movie.Id == criticReview.MovieId)?.Title;
        return View(criticReview);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string? movieTitle)
    {
        var criticReview = await _context.CriticReview
            .Where(review => review.Id == id)
            .FirstOrDefaultAsync();

        if (criticReview == null || criticReview.CreatedBy != _userId)
        {
            return NotFound();
        }

        if (await TryUpdateModelAsync(criticReview, "", review => review.MovieId, review => review.Review, review => review.Rating, review => review.IsPublished))
        {
            if (!await _context.Movie.AnyAsync(movie => movie.Id == criticReview.MovieId))
            {
                ModelState.AddModelError(nameof(CriticReview.MovieId), "Select a valid movie.");
            }

            if (!ModelState.IsValid)
            {
                await LoadMovieSelectList(criticReview.MovieId);
                ViewData["MovieTitle"] = movieTitle;
                return View(criticReview);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CriticReviewExists(criticReview.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        await LoadMovieSelectList(criticReview.MovieId);
        ViewData["MovieTitle"] = movieTitle;
        return View(criticReview);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var criticReview = await _context.CriticReview
            .Where(review => review.CreatedBy == _userId)
            .Include(review => review.Movie)
            .FirstOrDefaultAsync(review => review.Id == id);

        if (criticReview == null || criticReview.CreatedBy != _userId)
        {
            return NotFound();
        }

        return View(criticReview);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var criticReview = await _context.CriticReview
            .Where(review => review.CreatedBy == _userId)
            .FirstOrDefaultAsync(review => review.Id == id);

        if (criticReview != null)
        {
            _context.CriticReview.Remove(criticReview);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadMovieSelectList(int? selectedMovieId = null)
    {
        var movies = await _context.Movie
            .OrderBy(movie => movie.Title)
            .ToListAsync();

        ViewData["Movies"] = movies;
        ViewData["MovieId"] = new SelectList(movies, "Id", "Title", selectedMovieId);
    }

    private bool CriticReviewExists(int id)
    {
        return _context.CriticReview.Any(review => review.Id == id);
    }
}
