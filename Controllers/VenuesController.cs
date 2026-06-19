using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using EventEase.Data;

namespace EventEase.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AzureBlobStorageService _blobService;

        public VenuesController(ApplicationDbContext context, AzureBlobStorageService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                if (venue.ImageFile != null && venue.ImageFile.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await venue.ImageFile.CopyToAsync(memoryStream);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(venue.ImageFile.FileName)}";
                    venue.ImageUrl = await _blobService.UploadFileAsync(memoryStream.ToArray(), fileName);
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }
    }
}