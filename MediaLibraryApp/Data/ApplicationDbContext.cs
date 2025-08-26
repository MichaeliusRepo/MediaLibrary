using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MediaLibraryApp.Models;

namespace MediaLibraryApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MediaLibraryApp.Models.MusicEntry> MusicEntry { get; set; } = default!;

        /// Four steps to add a table:
        /// 1. Create a Model Class
        /// 2. Add DB Set
        /// 3. add-migration AddMusicEntryTable
        /// 4. update-database

        public DbSet<MusicEntry> MusicEntries { get; set; } = default!;
        public DbSet<MediaLibraryApp.Models.GameEntry> GameEntry { get; set; } = default!;

    }
}
