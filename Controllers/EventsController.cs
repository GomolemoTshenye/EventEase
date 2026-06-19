using EventEase.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;

public class EventsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly AzureBlobStorageService _blobService;

    public EventsController(ApplicationDbContext context, AzureBlobStorageService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    // GET: Events
    public async Task<IActionResult> Index(int? eventTypeFilter, string searchString)
    {
        var events = _context.Events
            .Include(e => e.EventType)
            .AsQueryable();

        if (eventTypeFilter.HasValue)
        {
            events = events.Where(e => e.EventTypeId == eventTypeFilter.Value);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            events = events.Where(e => e.Name.Contains(searchString) ||
                                       e.Description.Contains(searchString));
        }

        ViewBag.EventTypes = new SelectList(await _context.EventTypes.ToListAsync(), "Id", "Name");
        ViewData["CurrentFilter"] = searchString;
        ViewData["EventTypeFilter"] = eventTypeFilter;

        return View(await events.OrderBy(e => e.EventDate).ToListAsync());
    }

    // GET: Events/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var @event = await _context.Events
            .Include(e => e.EventType)
            .FirstOrDefaultAsync(m => m.Id == id);

        return @event == null ? NotFound() : View(@event);
    }

    // GET: Events/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.EventTypes = new SelectList(await _context.EventTypes.ToListAsync(), "Id", "Name");
        return View();
    }

    // POST: Events/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Event @event, IFormFile imageFile)
    {
        if (@event.EventDate <= DateTime.Now)
        {
            ModelState.AddModelError("EventDate", "Event date must be in the future");
        }

        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                @event.ImageUrl = await _blobService.UploadFileAsync(memoryStream.ToArray(), fileName);
            }

            _context.Add(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.EventTypes = new SelectList(await _context.EventTypes.ToListAsync(), "Id", "Name", @event.EventTypeId);
        return View(@event);
    }

    // GET: Events/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var @event = await _context.Events.FindAsync(id);
        if (@event == null)
            return NotFound();

        ViewBag.EventTypes = new SelectList(await _context.EventTypes.ToListAsync(), "Id", "Name", @event.EventTypeId);
        return View(@event);
    }

    // POST: Events/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Event @event, IFormFile imageFile)
    {
        if (id != @event.Id)
            return NotFound();

        if (@event.EventDate <= DateTime.Now)
        {
            ModelState.AddModelError("EventDate", "Event date must be in the future");
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(@event.ImageUrl))
                        await _blobService.DeleteFileAsync(@event.ImageUrl);

                    using var memoryStream = new MemoryStream();
                    await imageFile.CopyToAsync(memoryStream);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                    @event.ImageUrl = await _blobService.UploadFileAsync(memoryStream.ToArray(), fileName);
                }

                _context.Update(@event);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(@event.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewBag.EventTypes = new SelectList(await _context.EventTypes.ToListAsync(), "Id", "Name", @event.EventTypeId);
        return View(@event);
    }

    // GET: Events/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var @event = await _context.Events
            .Include(e => e.EventType)
            .FirstOrDefaultAsync(m => m.Id == id);

        return @event == null ? NotFound() : View(@event);
    }

    // POST: Events/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var @event = await _context.Events.FindAsync(id);
        if (@event != null)
        {
            if (!string.IsNullOrEmpty(@event.ImageUrl))
                await _blobService.DeleteFileAsync(@event.ImageUrl);

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool EventExists(int id)
    {
        return _context.Events.Any(e => e.Id == id);
    }
}