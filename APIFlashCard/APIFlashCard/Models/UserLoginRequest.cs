using System.ComponentModel.DataAnnotations;

namespace APIFlashCard.Models
{
    public class UserLoginRequest

    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public byte[] EncryptedPassword { get; set; }
    }
}