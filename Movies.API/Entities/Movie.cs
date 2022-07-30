using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Movies.API.Entities
{
    [Table("Movies")]
    public class Movie
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MinLength(10)]
        [MaxLength(2000)]
        public string Description { get; set; }

        [MaxLength(200)]
        public string Genre { get; set; }

        [Required]
        public DateTimeOffset ReleaseDate { get; set; }

        [Required]
        public Guid DirectorId { get; set; }
        public Director Director { get; set; }
    }
}
