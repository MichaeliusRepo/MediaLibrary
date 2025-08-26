using MediaLibraryApp.Data;
using MediaLibraryApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaLibraryApp.Controllers
{
    public class MusicEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MusicEntriesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: MusicEntries
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["TitleSortParam"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" :
                sortOrder == "Title" ? "title_desc" : "Title";
            ViewData["ArtistSortParam"] = sortOrder == "Artist" ? "artist_desc" : "Artist";
            ViewData["AlbumSortParam"] = sortOrder == "Album" ? "album_desc" : "Album";
            ViewData["GenreSortParam"] = sortOrder == "Genre" ? "genre_desc" : "Genre";
            ViewData["TagSortParam"] = sortOrder == "Tag" ? "tag_desc" : "Tag";
            ViewData["RatingSortParam"] = sortOrder == "Rating" ? "rating_desc" : "Rating";
            ViewData["CommentSortParam"] = sortOrder == "Comment" ? "comment_desc" : "Comment";


            var userId = _userManager.GetUserId(User);
            var userEntries = await _context.MusicEntries
                .Where(x => x.UserId == userId)
                .ToListAsync();


            var entries = sortOrder switch
            {
                "Comment" => userEntries.OrderBy(m => m.Comment),
                "comment_desc" => userEntries.OrderByDescending(m => m.Comment),

                "Rating" => userEntries.OrderBy(m => m.Rating),
                "rating_desc" => userEntries.OrderByDescending(m => m.Rating),

                "Tag" => userEntries.OrderBy(m => m.Tag),
                "tag_desc" => userEntries.OrderByDescending(m => m.Tag),

                "Genre" => userEntries.OrderBy(m => m.Genre),
                "genre_desc" => userEntries.OrderByDescending(m => m.Genre),

                "Album" => userEntries.OrderBy(m => m.Album),
                "album_desc" => userEntries.OrderByDescending(m => m.Album),

                "Artist" => userEntries.OrderBy(m => m.Artist),
                "artist_desc" => userEntries.OrderByDescending(m => m.Artist),

                "Title" => userEntries.OrderBy(m => m.Title),
                "title_desc" => userEntries.OrderByDescending(m => m.Title),
                _ => userEntries.OrderBy(m => m.Title) // default
            };

            return View(entries);
            //return View(await entries.ToListAsync());
            //return View(await entries.ToListAsync())


            //return View(userEntries);
            //return View(await _context.MusicEntry.ToListAsync());
        }

        // GET: MusicEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musicEntry = await _context.MusicEntry
                .FirstOrDefaultAsync(m => m.Id == id);
            if (musicEntry == null)
            {
                return NotFound();
            }

            return View(musicEntry);
        }

        // GET: MusicEntries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MusicEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Artist,Album,Genre,Tag,Rating,Comment")] MusicEntry musicEntry)
        {
            if (ModelState.IsValid)
            {
                // Set UserId to the current logged-in user's ID
                var userId = _userManager.GetUserId(User);
                if (userId == null || userId == string.Empty)
                    return Forbid();
                musicEntry.UserId = userId;


                _context.Add(musicEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
                PrintErrorsToConsole();


            return View(musicEntry);
        }

        // GET: MusicEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musicEntry = await _context.MusicEntry.FindAsync(id);
            if (musicEntry == null)
            {
                return NotFound();
            }

            // Check ownership
            var userId = _userManager.GetUserId(User);
            if (userId == null || userId == string.Empty || musicEntry.UserId != userId)
                return Forbid();

            return View(musicEntry);
        }

        // POST: MusicEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Artist,Album,Genre,Tag,Rating,Comment")] MusicEntry musicEntry)
        {
            if (id != musicEntry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check ownership
                    var userId = _userManager.GetUserId(User);
                    if (userId == null || userId == string.Empty)
                        return Forbid();
                    musicEntry.UserId = userId;



                    _context.Update(musicEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MusicEntryExists(musicEntry.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }



            return View(musicEntry);
        }

        // GET: MusicEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musicEntry = await _context.MusicEntry
                .FirstOrDefaultAsync(m => m.Id == id);
            if (musicEntry == null)
            {
                return NotFound();
            }

            // Check ownership
            var userId = _userManager.GetUserId(User);
            if (userId == null || userId == string.Empty || musicEntry.UserId != userId)
                return Forbid();

            return View(musicEntry);
        }

        // POST: MusicEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var musicEntry = await _context.MusicEntry.FindAsync(id);

            if (musicEntry != null)
            {
                // Check ownership
                var userId = _userManager.GetUserId(User);
                if (userId == null || userId == string.Empty || musicEntry.UserId != userId)
                    return Forbid();

                _context.MusicEntry.Remove(musicEntry);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MusicEntryExists(int id)
        {
            return _context.MusicEntry.Any(e => e.Id == id);
        }

        private void PrintErrorsToConsole()
        {
            foreach (var modelState in ModelState)
                foreach (var error in modelState.Value.Errors)
                    Console.WriteLine($"Field: {modelState.Key}, Error: {error.ErrorMessage}");
        }
    }
}
