using System.ComponentModel.DataAnnotations;

namespace APIFlashCard.Models
{
    public class TokenRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }
    }
}