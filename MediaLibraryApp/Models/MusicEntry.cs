using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MediaLibraryApp.Models
{
    public class MusicEntry
    {
        //[Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Artist { get; set; } = string.Empty;

        public string? Album { get; set; }
        public string? Genre { get; set; }
        public string? Tag { get; set; }

        [Range(1,100)]
        public int? Rating { get; set; }
        public string? Comment { get; set; }

        public string? UserId { get; set; } = string.Empty;


        /// Four steps to add a table:
        /// 1. Create a Model Class
        /// 2. Add DB Set
        /// 3. add-migration AddMusicEntryTable
        /// 4. update-database

    }
}
