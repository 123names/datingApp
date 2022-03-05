using System;
using System.ComponentModel.DataAnnotations;

namespace datingApp.api.DTOs
{
    public class RegisterDto
    {
        [Required] public string Username { get; set; }
        [Required] public string Email { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string KnownAs { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }
        [Required][StringLength(32, MinimumLength = 8)] public string Password { get; set; }
    }
}