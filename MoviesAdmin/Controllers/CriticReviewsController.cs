using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;

[Authorize(Roles = "administrator")]
public class CriticReviewsController : Controller
{
    private readonly MoviesAdminContext _context;

    public CriticReviewsController(MoviesAdminContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var criticReviews = await _context.CriticReview
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
            .Include(review => review.Movie)
            .FirstOrDefaultAsync(review => review.Id == id);

        if (criticReview == null)
        {
            return NotFound();
        }

        return View(criticReview);
    }

    public async Task<IActionResult> Create()
    {
        await LoadMovieSelectList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("MovieId,Review,Rating,IsPublished")] CriticReview criticReview)
    {
        criticReview.CreatedBy = User.GetUserId();
        criticReview.CreatedDate = DateTime.Now;
        ModelState.Remove(nameof(CriticReview.CreatedBy));

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
        return View(criticReview);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var criticReview = await _context.CriticReview.FindAsync(id);
        if (criticReview == null)
        {
            return NotFound();
        }

        await LoadMovieSelectList(criticReview.MovieId);
        return View(criticReview);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id)
    {
        var criticReview = await _context.CriticReview.FindAsync(id);
        if (criticReview == null)
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
        return View(criticReview);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var criticReview = await _context.CriticReview
            .Include(review => review.Movie)
            .FirstOrDefaultAsync(review => review.Id == id);

        if (criticReview == null)
        {
            return NotFound();
        }

        return View(criticReview);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var criticReview = await _context.CriticReview.FindAsync(id);
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

        ViewData["MovieId"] = new SelectList(movies, "Id", "Title", selectedMovieId);
    }

    private bool CriticReviewExists(int id)
    {
        return _context.CriticReview.Any(review => review.Id == id);
    }
}
