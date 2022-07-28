using System;
using System.ComponentModel.DataAnnotations;


namespace Movies.API.Models
{
    public class PosterForCreation
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public byte[] Bytes { get; set; }
    }
}
