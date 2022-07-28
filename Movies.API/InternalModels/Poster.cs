using System;
using System.ComponentModel.DataAnnotations;


namespace Movies.API.InternalModels
{
    public class Poster
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid MovieId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public byte[] Bytes { get; set; }
    }
}
