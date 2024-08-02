﻿using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO
    {
        public required string? Username { get; set; }

        [StringLength(8, MinimumLength = 4)]
        public required string Password { get; set; }

        public required string? KnownAs { get; set; }

        public required string? DateOfBirth { get; set; }

        public required string? Gender { get; set; }

        public required string? City { get; set; }

        public required string? Country { get; set; }
    }
}
