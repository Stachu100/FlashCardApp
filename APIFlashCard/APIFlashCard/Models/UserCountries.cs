using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIFlashCard.Models
{
    public class UserCountries
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_UserCountries { get; set; }

        [Required]
        public int ID_User { get; set; }

        [Required]
        public int ID_Country { get; set; }
    }
}