using System;
using System.ComponentModel.DataAnnotations;


namespace Movies.API.Models
{
    public class MovieForCreation
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(2000)]
        [MinLength(10)]
        public string Description { get; set; }

        [MaxLength(200)]
        public string Genre { get; set; }

        [Required]
        public DateTimeOffset? ReleaseDate { get; set; }

        [Required]
        public Guid? DirectorId { get; set; }
    }
}
