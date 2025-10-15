using System.ComponentModel.DataAnnotations;

namespace APIFlashCard.Models
{
    public class TokenCheck
    {
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}