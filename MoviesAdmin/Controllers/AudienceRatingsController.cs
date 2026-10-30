using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;

[Authorize(Roles = "administrator")]
public class AudienceRatingsController : Controller
{
    private readonly MoviesAdminContext _context;

    public AudienceRatingsController(MoviesAdminContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.AudienceRating
            .OrderBy(r => r.Title)
            .ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var audienceRating = await _context.AudienceRating
            .FirstOrDefaultAsync(r => r.Id == id);

        if (audienceRating == null)
        {
            return NotFound();
        }

        return View(audienceRating);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title")] AudienceRating audienceRating)
    {
        if (ModelState.IsValid)
        {
            _context.Add(audienceRating);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(audienceRating);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var audienceRating = await _context.AudienceRating.FindAsync(id);
        if (audienceRating == null)
        {
            return NotFound();
        }

        return View(audienceRating);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] AudienceRating audienceRating)
    {
        if (id != audienceRating.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(audienceRating);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AudienceRatingExists(audienceRating.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(audienceRating);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var audienceRating = await _context.AudienceRating
            .FirstOrDefaultAsync(r => r.Id == id);

        if (audienceRating == null)
        {
            return NotFound();
        }

        return View(audienceRating);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var audienceRating = await _context.AudienceRating.FindAsync(id);
        if (audienceRating != null)
        {
            if (await _context.Movie.AnyAsync(movie => movie.AudienceRatingId == id))
            {
                ModelState.AddModelError(string.Empty, "This audience rating cannot be deleted because it is assigned to one or more movies.");
                return View(audienceRating);
            }

            _context.AudienceRating.Remove(audienceRating);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AudienceRatingExists(int id)
    {
        return _context.AudienceRating.Any(r => r.Id == id);
    }
}
