using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIFlashCard.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Category { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [MaxLength(255)]
        public string CategoryName { get; set; }

        [Required]
        [MaxLength(255)]
        public string FrontLanguage { get; set; }

        [Required]
        [MaxLength(255)]
        public string BackLanguage { get; set; }

        [MaxLength(50)]
        public string LanguageLevel { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }
    }
}