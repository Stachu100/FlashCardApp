using System.ComponentModel.DataAnnotations;

namespace APIFlashCard.Models
{
    public class AdminPanelLogin
    {
        [Required]
        [MaxLength(255)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }
    }
}