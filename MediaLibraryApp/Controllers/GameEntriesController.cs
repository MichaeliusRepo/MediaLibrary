using MediaLibraryApp.Data;
using MediaLibraryApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaLibraryApp.Controllers
{
    public class GameEntriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public GameEntriesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GameEntries
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["TitleSortParam"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" :
    sortOrder == "Title" ? "title_desc" : "Title";
            ViewData["DeveloperSortParam"] = sortOrder == "Developer" ? "developer_desc" : "Developer";
            ViewData["PublisherSortParam"] = sortOrder == "Publisher" ? "publisher_desc" : "Publisher";
            ViewData["GenreSortParam"] = sortOrder == "Genre" ? "genre_desc" : "Genre";
            ViewData["TagSortParam"] = sortOrder == "Tag" ? "tag_desc" : "Tag";
            ViewData["RatingSortParam"] = sortOrder == "Rating" ? "rating_desc" : "Rating";
            ViewData["CommentSortParam"] = sortOrder == "Comment" ? "comment_desc" : "Comment";


            var userId = _userManager.GetUserId(User);
            var userEntries = await _context.GameEntry
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

                "Publisher" => userEntries.OrderBy(m => m.Publisher),
                "publisher_desc" => userEntries.OrderByDescending(m => m.Publisher),

                "Developer" => userEntries.OrderBy(m => m.Developer),
                "developer_desc" => userEntries.OrderByDescending(m => m.Developer),

                "Title" => userEntries.OrderBy(m => m.Title),
                "title_desc" => userEntries.OrderByDescending(m => m.Title),
                _ => userEntries.OrderBy(m => m.Title) // default
            };

            return View(entries);
            //return View(await _context.GameEntry.ToListAsync());
        }

        // GET: GameEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameEntry = await _context.GameEntry
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gameEntry == null)
            {
                return NotFound();
            }

            return View(gameEntry);
        }

        // GET: GameEntries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GameEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Developer,Publisher,Genre,Tag,Rating,Comment,UserId")] GameEntry gameEntry)
        {
            if (ModelState.IsValid)
            {
                // Set UserId to the current logged-in user's ID
                var userId = _userManager.GetUserId(User);
                if (userId == null || userId == string.Empty)
                    return Forbid();
                gameEntry.UserId = userId;

                _context.Add(gameEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
                PrintErrorsToConsole();


            return View(gameEntry);
        }

        // GET: GameEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameEntry = await _context.GameEntry.FindAsync(id);
            if (gameEntry == null)
            {
                return NotFound();
            }

            // Check ownership
            var userId = _userManager.GetUserId(User);
            if (userId == null || userId == string.Empty || gameEntry.UserId != userId)
                return Forbid();

            return View(gameEntry);
        }

        // POST: GameEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Developer,Publisher,Genre,Tag,Rating,Comment")] GameEntry gameEntry)
        {
            if (id != gameEntry.Id)
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
                    gameEntry.UserId = userId;

                    _context.Update(gameEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameEntryExists(gameEntry.Id))
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
            return View(gameEntry);
        }

        // GET: GameEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameEntry = await _context.GameEntry
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gameEntry == null)
            {
                return NotFound();
            }

            // Check ownership
            var userId = _userManager.GetUserId(User);
            if (userId == null || userId == string.Empty || gameEntry.UserId != userId)
                return Forbid();

            return View(gameEntry);
        }

        // POST: GameEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gameEntry = await _context.GameEntry.FindAsync(id);
            if (gameEntry != null)
            {
                // Check ownership
                var userId = _userManager.GetUserId(User);
                if (userId == null || userId == string.Empty || gameEntry.UserId != userId)
                    return Forbid();

                _context.GameEntry.Remove(gameEntry);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameEntryExists(int id)
        {
            return _context.GameEntry.Any(e => e.Id == id);
        }



        private void PrintErrorsToConsole()
        {
            foreach (var modelState in ModelState)
                foreach (var error in modelState.Value.Errors)
                    Console.WriteLine($"Field: {modelState.Key}, Error: {error.ErrorMessage}");
        }
    }
}
